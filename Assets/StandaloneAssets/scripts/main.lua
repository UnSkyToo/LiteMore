require 'framework.init'

local main = {}

function main:Startup()
	print('main start')
	Framework_Startup()
	self.lo = UIManager:OpenUI('dpsui', 'ui/dpsui.prefab')
	return true
end

function main:Shutdown()
	UIManager:CloseUI(self.lo)
	Framework_Shutdown()
	print('main stop')
end

function main:Tick(dt)
	Framework_Tick(dt)
	--print('main tick ' .. dt)
end

function main:EnterForeground()
	print('EnterForeground')
end

function main:EnterBackground()
	print('EnterBackground')
end

print('lite framework startup')
return main