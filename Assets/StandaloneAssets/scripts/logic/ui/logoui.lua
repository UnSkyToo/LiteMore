local LogoUI = BaseClass('LogoUI', UIBase)

function LogoUI:Ctor()
	self.DepthMode = UIDepthMode.Normal
	self.Depth = 0
end

function LogoUI:Dtor()
end

function LogoUI:OnOpen()
	print('LogoUI OnOpen')

	--local a = self._CSEntity_:FindChild('Tips/Value'):GetComponent(typeof(CS.UnityEngine.UI.Text))
	--a.text = "adsadasd"

	--local txt = self:FindChild('Tips/Value'):GetComponent(typeof(CS.UnityEngine.UI.Text))
	local txt = self:FindComponent('Tips/Value', UIText)
	txt.text = 'asd1123123'

	local mo = MotionRepeatSequence(MotionFadeOut(1), MotionFadeIn(1))
	self:ExecuteMotion(mo)


	self.a = 1
	self:AddEventToChild('Button', function(obj)
		print(obj.name .. self.a)
		self.a = self.a + 1
	end)

	self.aa = false
	self:AddEventToChild('Remove', function(obj)
		--self:RemoveEventFromChild('Button')
		self:EnableTouched('Button', self.aa)
		self.aa = not self.aa

		self:AbandonMotion(mo)
	end)

end

function LogoUI:OnClose()
	print('LogoUI OnClose')
end

return LogoUI