local DpsUI = BaseClass('DpsUI', BaseUI)

function DpsUI:Ctor()
	self.DepthMode = UIDepthMode.Normal
	self.Depth = 100

	--[[RegisterCSEvent('NpcAttrChangedEvent', self, function(_, evt)
	end)]]

	self.Interval_ = 0.5
    self.Time_ = 0
end

function DpsUI:Dtor()
	--UnRegisterAllCSEvent(self)
end

function DpsUI:OnOpen()
	ItemObj_ = self:FindChild("Item").gameObject;
    ItemObj_:SetActive(false)

    ContentRoot_ = self:FindChild("List/Viewport/Content")
    DpsText_ = self:GetComponent("Dps", UIText);

    self:AddEventToChild("BtnClear", function()
        CS.LiteMore.Player.PlayerManager.Dps:Clear()
        UIHelper.RemoveAllChildren(ContentRoot_)
    end)

	self:Refresh()
end

function DpsUI:OnClose()
end

function DpsUI:OnTick(dt)
	self.Time_ = self.Time_ + dt

    if self.Time_ >= self.Interval_ then
        self.Time_ = self.Time_ - self.Interval_
        self:Refresh()
    end
end

function DpsUI:Refresh()
	local Chunks = CS.LiteMore.Player.PlayerManager.Dps:GetChunks()

	for i = 0, ContentRoot_.childCount - 1 do
		ContentRoot_:GetChild(i).gameObject:SetActive(false)
	end

    for i = 0, Chunks.Count - 1 do
        local Child
        if i >= ContentRoot_.childCount then
        	Child = self:CreateItem()
        else
        	Child = ContentRoot_:GetChild(i)
        end

        Child.gameObject:SetActive(true);
        UIHelper.GetComponent(Child, "Name", UIText).text = Chunks[i].SourceName
        UIHelper.GetComponent(Child, "Text", UIText).text = string.format("%0.2f", Chunks[i].Value) .. "(" .. string.format("%0.2f", Chunks[i].Percent*100) .. "%)"
        UIHelper.GetComponent(Child, "Value", UISlider).value = Chunks[i].Percent
    end

    DpsText_.text = "Dps:" .. string.format("%0.2f", CS.LiteMore.Player.PlayerManager.Dps.Dps)
end

function DpsUI:CreateItem()
    local Obj = CS.UnityEngine.Object.Instantiate(ItemObj_)
    Obj.transform:SetParent(ContentRoot_, false)
    UIHelper.GetComponent(Obj.transform, "Value/Fill Area/Fill", UIImage).color = CS.UnityEngine.Color(CS.UnityEngine.Random.value, 0.8, CS.UnityEngine.Random.value)
    return Obj.transform
end

return DpsUI