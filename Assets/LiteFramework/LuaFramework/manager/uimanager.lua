UIManager = {}

local _OpenUI_ = CS.Lite.Framework.Lua.LuaRuntime.OpenLuaUI
local _CloseUI_ = CS.Lite.Framework.Lua.LuaRuntime.CloseLuaUI

function UIManager:OpenUI(uipath, prefabpath, ...)
	local ui = require('logic.ui.' .. uipath):Create(...)
	_OpenUI_(prefabpath, ui)
	return ui
end

function UIManager:CloseUI(ui)
	if ui == nil then
		return
	end
	
	_CloseUI_(ui)
	ui:Delete()
end