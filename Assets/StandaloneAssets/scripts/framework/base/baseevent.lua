BaseEvent = {}

function BaseEvent:Create(obj, s)
	local mt = {}

	mt.__index = function(t, k)
        if s[k] ~= nil then
            return s[k]
        end
        error('attempt to read non-existed value')
    end
	mt.__newindex = function(t, k, v)
		error('attempt to update a read-only table')
	end

	return setmetatable(obj, mt)
end