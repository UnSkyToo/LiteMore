UIBase = {}

--local _UIHelper = CS.Lite.Framework.UIHelper

function UIBase:FindChild(path)
	print(type(self._CSEntity_))
	return self._CSEntity_:FindChild(path)
end

function UIBase:FindComponent(path, ctype)
	return self._CSEntity_:FindComponent(path, ctype)
end

function UIBase:AddEvent(func, etype)
	etype = etype or UIEventType.Click
	self._CSEntity_:AddEvent(func, etype)
end

function UIBase:RemoveEvent(func, etype)
	etype = etype or UIEventType.Click
	self._CSEntity_:RemoveEvent(func, etype)
end

function UIBase:AddEventToChild(path, func, etype)
	etype = etype or UIEventType.Click
	self._CSEntity_:AddEventToChild(path, func, etype)
end

function UIBase:RemoveEventFromChild(path, func, etype)
	etype = etype or UIEventType.Click
	self._CSEntity_:RemoveEventFromChild(path, func, etype)
end

function UIBase:ShowChild(path)
	self._CSEntity_:ShowChild(path)
end

function UIBase:HideChild(path)
	self._CSEntity_:HideChild(path)
end

function UIBase:EnableTouched(enabled)
	self._CSEntity_:EnableTouched(enabled)
end

function UIBase:EnableTouched(path, enabled)
	self._CSEntity_:EnableTouched(path, enabled)
end

function UIBase:ExecuteMotion(motion)
	self._CSEntity_:ExecuteMotion(motion)
end

function UIBase:AbandonMotion(motion)
	self._CSEntity_:AbandonMotion(motion)
end