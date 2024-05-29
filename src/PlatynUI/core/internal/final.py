class Final(type):
    def __new__(mcs, name, bases, namespace):
        for b in bases:
            if isinstance(b, Final):
                raise TypeError("type '{0}' is not an acceptable base type".format(b.__name__))
        return type.__new__(mcs, name, bases, dict(namespace))
