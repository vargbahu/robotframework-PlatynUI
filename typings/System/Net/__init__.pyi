import typing, abc
from System import Array_1
from System.IO import TextWriter

class WebUtility(abc.ABC):
    @staticmethod
    def UrlDecode(encodedValue: typing.Optional[str]) -> typing.Optional[str]: ...
    @staticmethod
    def UrlDecodeToBytes(encodedValue: typing.Optional[Array_1[int]], offset: int, count: int) -> typing.Optional[Array_1[int]]: ...
    @staticmethod
    def UrlEncode(value: typing.Optional[str]) -> typing.Optional[str]: ...
    @staticmethod
    def UrlEncodeToBytes(value: typing.Optional[Array_1[int]], offset: int, count: int) -> typing.Optional[Array_1[int]]: ...
    # Skipped HtmlDecode due to it being static, abstract and generic.

    HtmlDecode : HtmlDecode_MethodGroup
    class HtmlDecode_MethodGroup:
        @typing.overload
        def __call__(self, value: typing.Optional[str]) -> str:...
        @typing.overload
        def __call__(self, value: typing.Optional[str], output: TextWriter) -> None:...

    # Skipped HtmlEncode due to it being static, abstract and generic.

    HtmlEncode : HtmlEncode_MethodGroup
    class HtmlEncode_MethodGroup:
        @typing.overload
        def __call__(self, value: typing.Optional[str]) -> str:...
        @typing.overload
        def __call__(self, value: typing.Optional[str], output: TextWriter) -> None:...


