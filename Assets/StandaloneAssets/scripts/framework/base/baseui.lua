BaseUI = {}

--local _UIHelper = CS.Lite.Framework.UIHelper

function BaseUI:FindChild(path)
	print(type(self._CSEntity_))
	return self._CSEntity_:FindChild(path)
end

function BaseUI:FindComponent(path, ctype)
	return self._CSEntity_:FindComponent(path, ctype)
end

function BaseUI:AddEvent(func, etype)
	etype = etype or UIEventType.Click
	self._CSEntity_:AddEvent(func, etype)
end

function BaseUI:RemoveEvent(func, etype)
	etype = etype or UIEventType.Click
	self._CSEntity_:RemoveEvent(func, etype)
end

function BaseUI:AddEventToChild(path, func, etype)
	etype = etype or UIEventType.Click
	self._CSEntity_:AddEventToChild(path, func, etype)
end

function BaseUI:RemoveEventFromChild(path, func, etype)
	etype = etype or UIEventType.Click
	self._CSEntity_:RemoveEventFromChild(path, func, etype)
end

function BaseUI:ShowChild(path)
	self._CSEntity_:ShowChild(path)
end

function BaseUI:HideChild(path)
	self._CSEntity_:HideChild(path)
end

function BaseUI:EnableTouched(enabled)
	self._CSEntity_:EnableTouched(enabled)
end

function BaseUI:EnableTouched(path, enabled)
	self._CSEntity_:EnableTouched(path, enabled)
end

function BaseUI:ExecuteMotion(motion)
	self._CSEntity_:ExecuteMotion(motion)
end

function BaseUI:AbandonMotion(motion)
	self._CSEntity_:AbandonMotion(motion)
end