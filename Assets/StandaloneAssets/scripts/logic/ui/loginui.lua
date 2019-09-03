local LoginUI = BaseClass('LoginUI', UIBase)

function LoginUI:Ctor()
	self.DepthMode = UIDepthMode.Normal
	self.Depth = 0
end

function LoginUI:Dtor()
end

function LoginUI:OnOpen()
end

function LoginUI:OnClose()
end

return LoginUI