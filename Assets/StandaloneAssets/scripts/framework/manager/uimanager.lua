UIManager = {}

local _OpenUI_ = CS.LiteFramework.Game.Lua.LuaRuntime.OpenLuaUI
local _CloseUI_ = CS.LiteFramework.Game.Lua.LuaRuntime.CloseLuaUI

function UIManager:OpenUI(uipath, desc, ...)
	local ui = require('logic.ui.' .. uipath):Create(...)

	if type(desc) == 'string' then
		desc = {PrefabName = desc, OpenMore = false, Cached = false}
	else
		desc = desc or {PrefabName = '', OpenMore = false, Cached = false}
		desc.OpenMore = desc.OpenMore or false
		desc.Cached = desc.Cached or false
	end
	
	_OpenUI_(desc, ui)
	return ui
end

function UIManager:CloseUI(ui)
	if ui == nil then
		return
	end
	
	_CloseUI_(ui)
	ui:Delete()
end