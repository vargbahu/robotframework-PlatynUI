import re
from typing import Any, Callable, Dict, List, Optional, Set, Type, TypeVar, cast, overload

from .adapter import Adapter, TStrategyBase
from .strategies import NativeProperties, Properties
from .strategybase import StrategyBase
from .technology import Technology

__all__ = [
    "AdapterProxy",
    "adapter_proxy_for",
    "AdapterProxyFactory",
    "WeightCalculator",
]


class AdapterProxy(Adapter):
    def __init__(self, adapter: Adapter):
        if adapter is None:
            raise Exception("adapter should not be None")

        self._adapter = adapter

    @property
    def adapter(self) -> Adapter:
        return self._adapter

    @property
    def valid(self) -> bool:
        return self.adapter.valid

    @property
    def id(self) -> str:
        return self.adapter.id

    @property
    def name(self) -> str:
        return self.adapter.name

    @property
    def class_name(self) -> str:
        return self.adapter.class_name

    @property
    def tag_name(self) -> str:
        return self.adapter.tag_name

    @property
    def role(self) -> str:
        return self.adapter.role

    @property
    def supported_roles(self) -> Set[str]:
        return self.adapter.supported_roles

    @property
    def type(self) -> str:
        return self.adapter.type

    @property
    def supported_types(self) -> Set[str]:
        return self.adapter.supported_types

    @property
    def framework_id(self) -> str:
        return self.adapter.framework_id

    @property
    def runtime_id(self) -> Any:
        return self.adapter.runtime_id

    @property
    def technology(self) -> Technology:
        return self.adapter.technology

    @property
    def _self_implemented_strategies(self) -> Set[str]:
        return set(
            [
                getattr(i, "strategy_name")
                for i in type(self).mro()
                if issubclass(i, StrategyBase) and i != StrategyBase and hasattr(i, "strategy_name")
            ]
        )

    @property
    def supported_strategies(self) -> Set[str]:
        return self._self_implemented_strategies.union(self.adapter.supported_strategies)

    @overload
    def get_strategy(self, strategy_type: Type[TStrategyBase]) -> TStrategyBase: ...

    @overload
    def get_strategy(self, strategy_type: Type[TStrategyBase], raise_exception: bool) -> Optional[TStrategyBase]: ...

    def get_strategy(self, strategy_type: Type[TStrategyBase], raise_exception: bool = True) -> Optional[TStrategyBase]:
        if hasattr(strategy_type, "strategy_name"):
            if strategy_type.strategy_name is not None:
                if strategy_type.strategy_name in self._self_implemented_strategies:
                    return cast(strategy_type, self)  # type: ignore

        return self.adapter.get_strategy(strategy_type, raise_exception)

    @property
    def parent(self) -> Optional["Adapter"]:
        return self.adapter.parent

    @property
    def children(self) -> List["Adapter"]:
        return self.adapter.children


class WeightCalculator:
    def __init__(self, adapter: Adapter):
        self.adapter = adapter
        self.cache: Dict[str, Any] = {}
        self.native_properties_cache: Dict[str, Any] = {}
        self.properties_cache: Dict[str, Any] = {}

    def cached(self, name: str) -> Any:
        if name in self.cache:
            return self.cache[name]

        if hasattr(self.adapter, name):
            self.cache[name] = getattr(self.adapter, name)
        else:
            self.cache[name] = None

        return self.cache[name]

    def properties_cached(self, name: str) -> Any:
        if name in self.properties_cache:
            return self.properties_cache[name]

        if Properties.strategy_name in self.adapter.supported_strategies:
            self.properties_cache[name] = self.adapter.get_strategy(Properties).get_property_value(name)
        else:
            self.properties_cache[name] = None

        return self.properties_cache[name]

    def native_properties_cached(self, name: str) -> Any:
        if name in self.native_properties_cache:
            return self.native_properties_cache[name]

        if NativeProperties.strategy_name in self.adapter.supported_strategies:
            self.native_properties_cache[name] = self.adapter.get_strategy(NativeProperties).get_native_property_value(
                name
            )
        else:
            self.native_properties_cache[name] = None

        return self.native_properties_cache[name]

    @staticmethod
    def test_values(actual: Any, expected: Any) -> bool:
        # TODO catch regular expressions errors?

        return re.fullmatch(expected, actual) is not None

    def calculate(self, criterias: Dict[str, object]) -> int:
        weight = 0

        if "role" in criterias and criterias["role"] is not None:
            if self.cached("role") == criterias["role"]:
                weight += 10000
            else:
                try:
                    i = list(self.cached("supported_roles")).index(criterias["role"])
                    weight += 5000 - i
                except ValueError:
                    return 0

        if "framework_id" in criterias and criterias["framework_id"] is not None:
            if self.test_values(self.cached("framework_id"), criterias["framework_id"]):
                weight += 1000
            else:
                return 0

        if "class_name" in criterias and criterias["class_name"] is not None:
            if self.test_values(self.cached("class_name"), criterias["class_name"]):
                weight += 500
            else:
                return 0

        if "tag_name" in criterias and criterias["tag_name"] is not None:
            if self.test_values(self.cached("tag_name"), criterias["tag_name"]):
                weight += 400
            else:
                return 0

        if "properties" in criterias and criterias["properties"] is not None:
            for p, v in cast(Dict[str, Any], criterias["properties"]).items():
                if self.test_values(self.properties_cached(p), v):
                    weight += 200
                else:
                    return 0

        if "native_properties" in criterias and criterias["native_properties"] is not None:
            for p, v in cast(Dict[str, Any], criterias["native_properties"]).items():
                if self.test_values(self.native_properties_cached(p), v):
                    weight += 200
                else:
                    return 0

        return weight


TAdapter = TypeVar("TAdapter", bound=Adapter)


class AdapterProxyFactory:
    class Entry:
        def __init__(self, proxy_type: Type[AdapterProxy], criterias: Dict[str, object]):
            self.proxy_type = proxy_type
            self.criterias = criterias

    registered_adapters = []  # type: List[Entry]

    @classmethod
    def register_proxy(cls, adapter_cls: Type[AdapterProxy], criterias: Dict[str, Any]) -> None:
        cls.registered_adapters.append(AdapterProxyFactory.Entry(adapter_cls, criterias.copy()))

    @classmethod
    def find_proxy_for(cls, adapter: TAdapter) -> TAdapter:
        weights = []
        calculator = WeightCalculator(adapter)

        for e in cls.registered_adapters:
            weights.append((calculator.calculate(e.criterias), e.proxy_type))

        if len(weights) > 0:
            last = sorted(weights, key=lambda x: x[0])[-1]

            if last[0] > 0:
                return cast(TAdapter, last[1](adapter))

        return adapter


def adapter_proxy_for(
    role: Optional[str] = None,
    framework_id: Optional[str] = None,
    class_name: Optional[str] = None,
    tag_name: Optional[str] = None,
    properties: Optional[Dict[str, str]] = None,
    native_properties: Optional[Dict[str, str]] = None,
    **decorator_kwargs: Any,
) -> Callable[[Type[AdapterProxy]], Type[AdapterProxy]]:

    def decorator(cls: Type[AdapterProxy]) -> Type[AdapterProxy]:

        AdapterProxyFactory.register_proxy(
            cls,
            {
                "role": role,
                "framework_id": framework_id,
                "class_name": class_name,
                "tag_name": tag_name,
                "properties": properties,
                "native_properties": native_properties,
                **decorator_kwargs,
            },
        )
        return cls

    return decorator
