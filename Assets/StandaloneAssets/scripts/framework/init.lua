require 'framework.extend.core'
require 'framework.base.baseclass'
require 'framework.extend.bitex'
require 'framework.extend.stringex'
require 'framework.extend.tableex'

require 'framework.base.eventdefined'
require 'framework.base.baseevent'
require 'framework.manager.eventmanager'

require 'framework.base.motiondefined'
require 'framework.base.uidefined'
require 'framework.base.baseui'
require 'framework.manager.uimanager'


function Framework_Startup()
	EventManager:Startup()
	return true
end

function Framework_Shutdown()
	EventManager:Shutdown()
end

function Framework_Tick(dt)
end