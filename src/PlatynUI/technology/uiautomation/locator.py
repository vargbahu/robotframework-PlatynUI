import collections
import enum
import numbers
import xml.sax.saxutils as xmlutils
from typing import Optional, Type, cast

from PlatynUI import ui
from PlatynUI.core import Adapter, ContextBase, LocatorBase, LocatorScope, TContextBase

from .technology import get_technology

__all__ = ["Locator", "locator", "LocatorScope"]


# noinspection PyPep8Naming
class Locator(LocatorBase):
    node = None  # type: str
    prefix = None  # type: str
    axis = None  # type: str
    path = None  # type: str
    __id = None  # type: str
    __name = None  # type: str
    __class_name = None  # type: str
    __role = None  # type: str
    __runtime_id = None  # type: str
    __framework_id = None  # type: str
    index = None  # type: int
    scope = None  # type: LocatorScope
    position = None  # type int

    __xpath_axis = {
        LocatorScope.Descendants: ".//",
        LocatorScope.Children: "",
        LocatorScope.Parent: "parent::",
        LocatorScope.Ancestor: "ancestor::",
        LocatorScope.AncestorOrSelf: "ancestor-or-self::",
        LocatorScope.DescendantsOrSelf: "descendant-or-self::",
        LocatorScope.Following: "following::",
        LocatorScope.FollowingSibling: "following-sibling::",
        LocatorScope.Preceding: "preceding::",
        LocatorScope.PrecedingSibling: "preceding-sibling::",
    }

    # noinspection PyShadowingBuiltins
    def __init__(
        self,
        *args,
        path: Optional[str] = None,
        id: Optional[str] = None,
        name: Optional[str] = None,
        class_name: Optional[str] = None,
        role: Optional[str] = None,
        prefix: Optional[str] = None,
        runtime_id: Optional[str] = None,
        framework_id: Optional[str] = None,
        index: Optional[int] = None,
        node: Optional[str] = None,
        scope: Optional[LocatorScope] = None,
        axis: Optional[str] = None,
        position: Optional[int] = None,
        **kwargs,
    ):

        super().__init__()

        self.custom_attributes = list(args)
        self.attributes = dict(kwargs)

        self.path = path
        self.node = node
        self.prefix = prefix

        self.axis = axis

        if id is not None:
            self.id = id

        if name is not None:
            self.name = name

        if class_name is not None:
            self.class_name = class_name

        if role is not None:
            self.role = role

        if runtime_id is not None:
            self.runtime_id = runtime_id

        if framework_id is not None:
            self.framework_id = framework_id

        self.index = index
        self.position = position
        self.scope = scope

    @property
    def id(self) -> str:
        return self.__id

    @id.setter
    def id(self, v: str):
        self.__id = v
        if v is None:
            self.attributes.pop("Id", None)
        else:
            self.attributes["Id"] = self.__id

    @property
    def name(self) -> str:
        return self.__name

    @name.setter
    def name(self, v: str):
        self.__name = v
        if v is None:
            self.attributes.pop("Name", None)
        else:
            self.attributes["Name"] = self.__name

    @property
    def class_name(self) -> str:
        return self.__class_name

    @class_name.setter
    def class_name(self, v: str):
        self.__class_name = v
        if v is None:
            self.attributes.pop("ClassName", None)
        else:
            self.attributes["ClassName"] = self.__class_name

    @property
    def role(self) -> str:
        return self.__role

    @role.setter
    def role(self, v: str):
        self.__role = v
        if v is None:
            self.attributes.pop("Role", None)
        else:
            self.attributes["Role"] = self.__role

    @property
    def framework_id(self) -> str:
        return self.__framework_id

    @framework_id.setter
    def framework_id(self, v: str):
        self.__framework_id = v
        if v is None:
            self.attributes.pop("FrameWorkId", None)
        else:
            self.attributes["FrameWorkId"] = self.__framework_id

    @property
    def runtime_id(self) -> str:
        return self.__runtime_id

    @runtime_id.setter
    def runtime_id(self, v: str):
        self.__runtime_id = v
        if v is None:
            self.attributes.pop("RuntimeId", None)
        else:
            self.attributes["RuntimeId"] = self.__runtime_id

    @property
    def display_name(self):
        return self.__display_name

    @display_name.setter
    def display_name(self, v: str):
        self.__display_name = v
        if self.context is not None:
            self.context.invalidate()

    @property
    def host(self):
        return self.__host

    @host.setter
    def host(self, v: str):
        self.__host = v
        if self.context is not None:
            self.context.invalidate()

    def _calc_display_name(self):
        if self.display_name is not None:
            return self.display_name

        parent_locator = self.get_parent_locator()
        if parent_locator is not None:
            return parent_locator._calc_display_name()

        return None

    def _calc_host(self):
        if self.host is not None:
            return self.host

        parent_locator = self.get_parent_locator()
        if parent_locator is not None:
            return parent_locator._calc_host()

        return None

    def _create_technology(self):
        return get_technology(self._calc_display_name(), self._calc_host())

    def create_context(self, context_parent: ContextBase, context_type: Type[TContextBase]) -> TContextBase:
        if context_type is None:
            context_type = ui.Element

        if isinstance(context_parent, ui.DesktopBase):
            if isinstance(context_parent.locator, Locator):
                parent_locator = cast(Locator, context_parent.locator)
                self.host = parent_locator.host
                self.display_name = parent_locator.display_name
            context_parent = None

        return super().create_context(context_parent=context_parent, context_type=context_type)

    def __repr__(self):
        if self.__last_path is not None:
            params = "path=%s" % repr(self.__last_path)
        else:
            params = ""

            if self.path is not None:
                if params != "":
                    params += ", "
                params += "path=%s" % repr(self.path)

            if self.axis is not None:
                if params != "":
                    params += ", "
                params += "axis=%s" % repr(self.axis)

            if self.node is not None:
                if params != "":
                    params += ", "
                params += "node=%s" % repr(self.node)

            if self.prefix is not None:
                if params != "":
                    params += ", "
                params += "prefix=%s" % repr(self.prefix)

            for v in self.custom_attributes:
                if v is not None:
                    if params != "":
                        params += ", "
                    params += '"%s"' % str(v)

            for n, v in self.attributes.items():
                if v is not None:
                    if params != "":
                        params += ", "
                    params += "%s=%s" % (n, repr(v))

            if self.scope is not None:
                if params != "":
                    params += ", "
                params += "scope=%s" % repr(self.scope)

            if self.position is not None:
                if params != "":
                    params += ", "
                params += "position=%s" % repr(self.position)

        if self.display_name is not None:
            if params != "":
                params += ", "
            params += "display_name=%s" % repr(self.display_name)

        if self.host is not None:
            if params != "":
                params += ", "
            params += "host=%s" % repr(self.host)

        return "locator(%s)" % params

    __last_path = None

    __last_parent = None
    __last_context_type = None
    __last_attributes = None
    __last_custom_attributes = None

    def get_path(self, parent, context_type: type) -> str:

        combined = (
            self.copy_from(getattr(context_type, "_locator"))
            if context_type is not None and hasattr(context_type, "_locator")
            else self
        )

        if (
            combined.__last_path is not None
            and combined.__last_parent == parent
            and combined.__last_context_type == context_type
            and combined.attributes == combined.__last_attributes
            and combined.__last_custom_attributes == combined.custom_attributes
        ):
            return combined.__last_path

        combined.__last_parent = parent
        combined.__last_context_type = context_type
        combined.__last_attributes = combined.attributes.copy()
        combined.__last_custom_attributes = combined.custom_attributes.copy()

        result = ""

        real_path = combined.path if combined.path is not None else None

        if real_path is not None:
            result = real_path
        else:
            attrs = ""

            for n, v in combined.attributes.items():
                if v is not None:
                    if attrs != "":
                        attrs += " and "
                    attrs += "@%s=%s" % (n, combined.xquery_repr(v))

            for v in combined.custom_attributes:
                if v is not None:
                    if attrs != "":
                        attrs += " and "
                    attrs += str(v)

            if combined.position is not None:
                if attrs != "":
                    attrs += " and "
                attrs += "position()=%s" % combined.position

            if combined.node is not None:
                result = combined.node + result
            elif hasattr(context_type, "default_role") and context_type.default_role is not None:
                result = context_type.default_role + result
            else:
                result = "*" + result

            if combined.prefix is not None:
                result = combined.prefix + ":" + result
            elif hasattr(context_type, "default_prefix") and context_type.default_prefix is not None:
                result = context_type.default_prefix + ":" + result

            if combined.axis is not None:
                result = combined.axis + result
            else:
                real_scope = combined.scope
                if real_scope is None:
                    if parent is None:
                        real_scope = LocatorScope.Children
                    else:
                        real_scope = (
                            LocatorScope.Children
                            if isinstance(parent, (ui.Application, ui.DesktopBase))
                            else LocatorScope.Descendants
                        )

                result = combined.__xpath_axis[real_scope] + result

            if attrs != "":
                attrs = "[%s]" % attrs
            if combined.index is not None:
                attrs += "[%s]" % combined.index

            result += attrs

        self.__last_path = result

        return result

    def copy_from(self, other: LocatorBase) -> Optional[LocatorBase]:
        if other is None:
            return self

        if isinstance(other, type(self)):
            for n in [
                "path",
                "id",
                "name",
                "class_name",
                "role",
                "prefix",
                "framework_id",
                "index",
                "node",
                "axis",
                "scope",
                "runtime_id",
                "position",
            ]:

                v = getattr(self, n)
                if v is None:
                    setattr(self, n, getattr(other, n))

            o = cast(Locator, other)

            if o.__display_name is not None:
                self.__display_name = o.__display_name
            if o.__host is not None:
                self.__host = o.__host
            if o._technology is not None:
                self._technology = o._technology

            for a in o.custom_attributes:
                if a not in self.custom_attributes:
                    self.custom_attributes.append(a)

            for a in o.attributes:
                if a not in self.attributes:
                    self.attributes[a] = o.attributes[a]

        return self

    def copy(self) -> Optional["LocatorBase"]:
        # return copy.deepcopy(self)
        result = Locator()

        result.__display_name = self.__display_name
        result.__host = self.__host
        result._technology = self._technology

        result.node = self.node
        result.prefix = self.prefix
        result.axis = self.axis
        result.path = self.path
        result.name = self.name
        result.id = self.id
        result.class_name = self.class_name
        result.role = self.role
        result.framework_id = self.framework_id
        result.index = self.index
        result.scope = self.scope
        result.position = self.position
        result.runtime_id = self.runtime_id
        result.attributes = self.attributes.copy()
        result.custom_attributes = self.custom_attributes.copy()

        return result

    def create_children_locator(self, *args, **kwargs) -> LocatorBase:
        return Locator(*args, **kwargs)

    def make_unique_locator(self, adapter: Adapter) -> LocatorBase:
        result = self.copy()
        result.runtime_id = adapter.runtime_id
        return result

    def create_child_locator(self, adapter: Adapter) -> LocatorBase:
        return Locator(scope=LocatorScope.Descendants, runtime_id=adapter.runtime_id)

    def create_parent_locator(self, adapter: Adapter) -> LocatorBase:
        return Locator(path="..")

    def create_top_level_locator(self, adapter: "Adapter") -> Optional["LocatorBase"]:
        return Locator(runtime_id=adapter.runtime_id, scope=LocatorScope.AncestorOrSelf)

    @staticmethod
    def xquery_repr(v) -> str:
        if isinstance(v, enum.Enum):
            return '"' + repr(v.value) + '"'
        if isinstance(v, bool):
            return repr(v).lower() + "()"
        if isinstance(v, str):
            return xmlutils.quoteattr(v)
        if isinstance(v, numbers.Number):
            return repr(v)
        if isinstance(v, dict):
            return "(" + ", ".join([Locator.xquery_repr(c) for c in v.items()]) + ")"
        if isinstance(v, collections.Iterable):
            return "(" + ", ".join([Locator.xquery_repr(c) for c in v]) + ")"
        return xmlutils.quoteattr(repr(v))


# noinspection PyShadowingBuiltins
def locator(
    *args,
    path: Optional[str] = None,
    id: str = None,
    name: str = None,
    class_name: str = None,
    role: str = None,
    prefix: str = None,
    runtime_id: str = None,
    framework_id: str = None,
    index: int = None,
    node: str = None,
    scope: LocatorScope = None,
    axis: str = None,
    position: int = None,
    display_name: str = None,
    host: str = None,
    **kwargs,
) -> Locator:
    return Locator(
        *args,
        path=path,
        id=id,
        name=name,
        class_name=class_name,
        role=role,
        prefix=prefix,
        runtime_id=runtime_id,
        framework_id=framework_id,
        index=index,
        node=node,
        scope=scope,
        axis=axis,
        position=position,
        display_name=display_name,
        host=host,
        **kwargs,
    )


# def locator(*args, **kwargs) -> Locator:
#     return Locator(*args, **kwargs)
