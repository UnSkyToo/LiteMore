require 'framework.init'

local main = {}

function main:Startup()
	print('main start')
	self.lo = UIManager:OpenUI('loadingui', 'loadingui')
	return true
end

function main:Shutdown()
	UIManager:CloseUI(self.lo)
	print('main stop')
end

function main:Tick(dt)
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