function BindHandler(obj, method)
    return function(...)
        return method(obj, ...)
    end
end

function BaseClass(classname, base)  
    local cls = {}  
    if base then
        setmetatable(cls, {__index = base})
        cls.base = base
    end
  
    cls.Name = classname
    cls.__index = cls

    cls.New = function(...)
        local instance = setmetatable({}, cls)
        local create
        create = function(c, ...)
             if c.base then
                  create(c.base, ...)
             end
             if c.Ctor then
                  c.Ctor(c, ...)
             end
        end
        instance.class = cls
        cls.realize = instance
        create(instance, ...)
        return instance
    end
    cls.Del = function()
        if not cls.realize then
            return
        end
    	local delete
        delete = function(c)
             if c.Dtor then
                  c.Dtor(c)
             end
             if c.base then
                  delete(c.base)
             end
        end
        delete(cls.realize)
    end

    cls.Create = function(_, ...)
        return cls.New(...)
    end
    cls.Delete = function(_)
        cls.Del()
    end

    return cls  
end

--[[local _Class_Holder_ = {}
function BaseClassEx(base)
    local class_type = {}
    class_type.Ctor = false
    class_type.base = base
    class_type.New =  function(...)
        local obj = {}
        local create
        create = function(c, ...)
            if c.base then
                create(c.base, ...)
            end
            if c.ctor then
                c.ctor(obj, ...)
            end
        end

        create(class_type, ...)
        setmetatable(obj, { __index = _Class_Holder_[class_type] })
        return obj
    end

    local vtbl = {}
    _Class_Holder_[class_type] = vtbl

    setmetatable(class_type,{__newindex =
        function(t,k,v)
            vtbl[k]=v
        end
    })
    
    if base then
        setmetatable(vtbl, {__index =
            function(t, k)
                local ret = _Class_Holder_[base][k]
                vtbl[k] = ret
                return ret
            end
        })
    end

    return class_type
end]]