EventManager = {}

local eventHandlerList = {}

function EventManager:Startup()
	eventHandlerList = {}
end

function EventManager:Shutdown()
	for k, v in pairs(eventHandlerList) do
        if table.len(v) ~= 0 then
		    print('EventManager : not unregister [' .. k .. ']')
        end
	end
	
	eventHandlerList = {}
end

function EventManager:Register(eventName, eventHandler)
	if type(eventName) ~= 'string' or type(eventHandler) ~= 'function' then
		return
	end

	if eventHandlerList[eventName] == nil then
		eventHandlerList[eventName] = {}
	end

	for k, v in pairs(eventHandlerList[eventName]) do
		if v == eventHandler then
			return
		end
	end

	table.insert(eventHandlerList[eventName], eventHandler)
end

function EventManager:UnRegister(eventName, eventHandler)
	if type(eventName) ~= 'string' or type(eventHandler) ~= 'function' then
		return
	end

	if eventHandlerList[eventName] == nil then
		return
	end

	for k, v in pairs(eventHandlerList[eventName]) do
		if eventHandler == v then
			table.remove(eventHandlerList[eventName], k)
			return
		end
	end
end

function EventManager:Send(event)
	if type (event.name) ~= 'string' then
		return
	end

	if eventHandlerList[event.name] == nil then
		return
	end

	for k, v in pairs(eventHandlerList[event.name]) do
		v(event)
	end
end