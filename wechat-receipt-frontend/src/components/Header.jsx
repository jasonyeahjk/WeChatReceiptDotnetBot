import { useState } from 'react'
import { Button } from '@/components/ui/button.jsx'
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar.jsx'
import { 
  DropdownMenu, 
  DropdownMenuContent, 
  DropdownMenuItem, 
  DropdownMenuTrigger 
} from '@/components/ui/dropdown-menu.jsx'
import { 
  User, 
  Settings, 
  LogOut, 
  Wallet, 
  Menu,
  X
} from 'lucide-react'

const Header = ({ user, onLogin, onLogout }) => {
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false)

  return (
    <header className="bg-white shadow-sm border-b sticky top-0 z-50">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between items-center h-16">
          {/* Logo and Brand */}
          <div className="flex items-center">
            <div className="flex-shrink-0">
              <h1 className="text-xl font-bold text-gray-900">
                微信拼团记账助手
              </h1>
            </div>
          </div>

          {/* Desktop Navigation */}
          <div className="hidden md:block">
            <div className="ml-10 flex items-baseline space-x-4">
              <a href="#dashboard" className="text-gray-900 hover:text-blue-600 px-3 py-2 rounded-md text-sm font-medium">
                仪表板
              </a>
              <a href="#groups" className="text-gray-500 hover:text-gray-900 px-3 py-2 rounded-md text-sm font-medium">
                拼团管理
              </a>
              <a href="#bills" className="text-gray-500 hover:text-gray-900 px-3 py-2 rounded-md text-sm font-medium">
                账本管理
              </a>
              <a href="#transactions" className="text-gray-500 hover:text-gray-900 px-3 py-2 rounded-md text-sm font-medium">
                交易记录
              </a>
            </div>
          </div>

          {/* User Menu */}
          <div className="hidden md:block">
            <div className="ml-4 flex items-center md:ml-6">
              {user ? (
                <DropdownMenu>
                  <DropdownMenuTrigger asChild>
                    <Button variant="ghost" className="relative h-8 w-8 rounded-full">
                      <Avatar className="h-8 w-8">
                        <AvatarImage src={user.avatarUrl} alt={user.nickname} />
                        <AvatarFallback>{user.nickname?.charAt(0) || 'U'}</AvatarFallback>
                      </Avatar>
                    </Button>
                  </DropdownMenuTrigger>
                  <DropdownMenuContent className="w-56" align="end" forceMount>
                    <div className="flex items-center justify-start gap-2 p-2">
                      <div className="flex flex-col space-y-1 leading-none">
                        <p className="font-medium">{user.nickname || user.username}</p>
                        <p className="w-[200px] truncate text-sm text-muted-foreground">
                          {user.walletAddress && `${user.walletAddress.slice(0, 6)}...${user.walletAddress.slice(-4)}`}
                        </p>
                      </div>
                    </div>
                    <DropdownMenuItem>
                      <User className="mr-2 h-4 w-4" />
                      <span>个人资料</span>
                    </DropdownMenuItem>
                    <DropdownMenuItem>
                      <Wallet className="mr-2 h-4 w-4" />
                      <span>钱包管理</span>
                    </DropdownMenuItem>
                    <DropdownMenuItem>
                      <Settings className="mr-2 h-4 w-4" />
                      <span>设置</span>
                    </DropdownMenuItem>
                    <DropdownMenuItem onClick={onLogout}>
                      <LogOut className="mr-2 h-4 w-4" />
                      <span>退出登录</span>
                    </DropdownMenuItem>
                  </DropdownMenuContent>
                </DropdownMenu>
              ) : (
                <Button onClick={onLogin} className="bg-blue-600 hover:bg-blue-700">
                  登录
                </Button>
              )}
            </div>
          </div>

          {/* Mobile menu button */}
          <div className="md:hidden">
            <Button
              variant="ghost"
              size="sm"
              onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)}
            >
              {isMobileMenuOpen ? (
                <X className="h-6 w-6" />
              ) : (
                <Menu className="h-6 w-6" />
              )}
            </Button>
          </div>
        </div>

        {/* Mobile Navigation */}
        {isMobileMenuOpen && (
          <div className="md:hidden">
            <div className="px-2 pt-2 pb-3 space-y-1 sm:px-3">
              <a href="#dashboard" className="text-gray-900 hover:text-blue-600 block px-3 py-2 rounded-md text-base font-medium">
                仪表板
              </a>
              <a href="#groups" className="text-gray-500 hover:text-gray-900 block px-3 py-2 rounded-md text-base font-medium">
                拼团管理
              </a>
              <a href="#bills" className="text-gray-500 hover:text-gray-900 block px-3 py-2 rounded-md text-base font-medium">
                账本管理
              </a>
              <a href="#transactions" className="text-gray-500 hover:text-gray-900 block px-3 py-2 rounded-md text-base font-medium">
                交易记录
              </a>
              {user ? (
                <div className="pt-4 pb-3 border-t border-gray-200">
                  <div className="flex items-center px-3">
                    <Avatar className="h-10 w-10">
                      <AvatarImage src={user.avatarUrl} alt={user.nickname} />
                      <AvatarFallback>{user.nickname?.charAt(0) || 'U'}</AvatarFallback>
                    </Avatar>
                    <div className="ml-3">
                      <div className="text-base font-medium text-gray-800">{user.nickname || user.username}</div>
                      <div className="text-sm font-medium text-gray-500">
                        {user.walletAddress && `${user.walletAddress.slice(0, 6)}...${user.walletAddress.slice(-4)}`}
                      </div>
                    </div>
                  </div>
                  <div className="mt-3 space-y-1">
                    <a href="#profile" className="block px-3 py-2 rounded-md text-base font-medium text-gray-400 hover:text-gray-500">
                      个人资料
                    </a>
                    <a href="#wallet" className="block px-3 py-2 rounded-md text-base font-medium text-gray-400 hover:text-gray-500">
                      钱包管理
                    </a>
                    <a href="#settings" className="block px-3 py-2 rounded-md text-base font-medium text-gray-400 hover:text-gray-500">
                      设置
                    </a>
                    <button 
                      onClick={onLogout}
                      className="block w-full text-left px-3 py-2 rounded-md text-base font-medium text-gray-400 hover:text-gray-500"
                    >
                      退出登录
                    </button>
                  </div>
                </div>
              ) : (
                <div className="pt-4 pb-3 border-t border-gray-200">
                  <Button onClick={onLogin} className="w-full bg-blue-600 hover:bg-blue-700">
                    登录
                  </Button>
                </div>
              )}
            </div>
          </div>
        )}
      </div>
    </header>
  )
}

export default Header

