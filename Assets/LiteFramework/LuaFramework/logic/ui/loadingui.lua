local LoadingUI = BaseClass('LoadingUI', UIBase)

function LoadingUI:Ctor()
	self.DepthMode = UIDepthMode.Normal
	self.Depth = 0
end

function LoadingUI:Dtor()
end

function LoadingUI:OnOpen()
	self.bar_ = self:FindComponent('bar', UISlider)
	self.tips_ = self:FindComponent('tips', UIText)

	self.bar_.value = 0
	self.tips_.text = 'loading.'
	self.tipstext_ =
	{
		[1] = '.',
		[2] = '..',
		[3] = '...',
	}
	self.tipsindex_ = 1
	self.frame_ = 1
end

function LoadingUI:OnClose()
end

function LoadingUI:OnTick(dt)
	self.bar_.value = self.bar_.value + 0.001
	self.frame_ = self.frame_ + 1

	if self.frame_ == 30 then
		self.frame_ = 0
		self.tipsindex_ = self.tipsindex_ + 1
		if self.tipsindex_ > 3 then
			self.tipsindex_ = 1
		end
		self.tips_.text = 'loading' .. self.tipstext_[self.tipsindex_]
	end
end

return LoadingUI