import { useState } from 'react'
import Header from './components/Header.jsx'
import Dashboard from './components/Dashboard.jsx'
import GroupManagement from './components/GroupManagement.jsx'
import ReceiptScanner from './components/ReceiptScanner.jsx'
import './App.css'

function App() {
  const [currentView, setCurrentView] = useState('dashboard')
  const [user, setUser] = useState(null)
  const [isReceiptScannerOpen, setIsReceiptScannerOpen] = useState(false)

  // Mock user for demo
  const mockUser = {
    userId: '1',
    username: 'demo_user',
    nickname: '演示用户',
    avatarUrl: '',
    walletAddress: '0x1234567890abcdef1234567890abcdef12345678'
  }

  const handleLogin = () => {
    setUser(mockUser)
  }

  const handleLogout = () => {
    setUser(null)
    setCurrentView('dashboard')
  }

  const handleReceiptSave = (transactionData) => {
    console.log('保存交易数据:', transactionData)
    // In real app, send to API
    alert('交易记录已保存！')
  }

  const renderCurrentView = () => {
    if (!user) {
      return (
        <div className="min-h-screen bg-gray-50 flex items-center justify-center">
          <div className="max-w-md w-full bg-white rounded-lg shadow-md p-8 text-center">
            <h2 className="text-2xl font-bold text-gray-900 mb-4">
              欢迎使用微信拼团记账助手
            </h2>
            <p className="text-gray-600 mb-6">
              智能记账，轻松分摊，让拼团费用管理变得简单
            </p>
            <button
              onClick={handleLogin}
              className="w-full bg-blue-600 text-white py-2 px-4 rounded-md hover:bg-blue-700 transition-colors"
            >
              立即登录
            </button>
          </div>
        </div>
      )
    }

    switch (currentView) {
      case 'groups':
        return <GroupManagement />
      case 'bills':
        return (
          <div className="text-center py-12">
            <h2 className="text-2xl font-bold mb-4">账本管理</h2>
            <p className="text-gray-600">功能开发中...</p>
          </div>
        )
      case 'transactions':
        return (
          <div className="text-center py-12">
            <h2 className="text-2xl font-bold mb-4">交易记录</h2>
            <p className="text-gray-600">功能开发中...</p>
          </div>
        )
      default:
        return <Dashboard user={user} />
    }
  }

  // Handle navigation from header
  const handleNavigation = (view) => {
    setCurrentView(view)
  }

  return (
    <div className="min-h-screen bg-gray-50">
      <Header 
        user={user} 
        onLogin={handleLogin} 
        onLogout={handleLogout}
      />
      
      {user && (
        <nav className="bg-white shadow-sm border-b">
          <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
            <div className="flex space-x-8">
              <button
                onClick={() => handleNavigation('dashboard')}
                className={`py-4 px-1 border-b-2 font-medium text-sm ${
                  currentView === 'dashboard'
                    ? 'border-blue-500 text-blue-600'
                    : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
                }`}
              >
                仪表板
              </button>
              <button
                onClick={() => handleNavigation('groups')}
                className={`py-4 px-1 border-b-2 font-medium text-sm ${
                  currentView === 'groups'
                    ? 'border-blue-500 text-blue-600'
                    : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
                }`}
              >
                拼团管理
              </button>
              <button
                onClick={() => handleNavigation('bills')}
                className={`py-4 px-1 border-b-2 font-medium text-sm ${
                  currentView === 'bills'
                    ? 'border-blue-500 text-blue-600'
                    : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
                }`}
              >
                账本管理
              </button>
              <button
                onClick={() => handleNavigation('transactions')}
                className={`py-4 px-1 border-b-2 font-medium text-sm ${
                  currentView === 'transactions'
                    ? 'border-blue-500 text-blue-600'
                    : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
                }`}
              >
                交易记录
              </button>
              <button
                onClick={() => setIsReceiptScannerOpen(true)}
                className="py-4 px-1 border-b-2 border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300 font-medium text-sm"
              >
                扫描收据
              </button>
            </div>
          </div>
        </nav>
      )}

      <main className="max-w-7xl mx-auto py-6 sm:px-6 lg:px-8">
        <div className="px-4 py-6 sm:px-0">
          {renderCurrentView()}
        </div>
      </main>

      <ReceiptScanner
        isOpen={isReceiptScannerOpen}
        onClose={() => setIsReceiptScannerOpen(false)}
        onSave={handleReceiptSave}
      />
    </div>
  )
}

export default App
