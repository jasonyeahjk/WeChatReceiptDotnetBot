import { useState, useEffect } from 'react'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card.jsx'
import { Button } from '@/components/ui/button.jsx'
import { Badge } from '@/components/ui/badge.jsx'
import { 
  Users, 
  Receipt, 
  Wallet, 
  TrendingUp,
  Plus,
  Camera,
  Upload,
  QrCode
} from 'lucide-react'

const Dashboard = ({ user }) => {
  const [stats, setStats] = useState({
    totalGroups: 0,
    activeBills: 0,
    totalBalance: 0,
    pendingTransactions: 0
  })

  const [recentActivities, setRecentActivities] = useState([])

  useEffect(() => {
    // Mock data - in real app, fetch from API
    setStats({
      totalGroups: 3,
      activeBills: 5,
      totalBalance: 1250.50,
      pendingTransactions: 2
    })

    setRecentActivities([
      {
        id: 1,
        type: 'expense',
        description: '午餐聚餐',
        amount: 128.50,
        group: '同事聚餐群',
        date: '2025-01-19',
        status: 'pending'
      },
      {
        id: 2,
        type: 'payment',
        description: '支付给张三',
        amount: 50.00,
        group: '周末出游',
        date: '2025-01-18',
        status: 'completed'
      },
      {
        id: 3,
        type: 'expense',
        description: '电影票',
        amount: 80.00,
        group: '朋友聚会',
        date: '2025-01-17',
        status: 'settled'
      }
    ])
  }, [])

  const quickActions = [
    {
      title: '扫码记账',
      description: '扫描收据快速记账',
      icon: QrCode,
      action: () => console.log('扫码记账'),
      color: 'bg-blue-500'
    },
    {
      title: '拍照记账',
      description: '拍摄收据自动识别',
      icon: Camera,
      action: () => console.log('拍照记账'),
      color: 'bg-green-500'
    },
    {
      title: '手动记账',
      description: '手动输入交易信息',
      icon: Plus,
      action: () => console.log('手动记账'),
      color: 'bg-purple-500'
    },
    {
      title: '上传支付记录',
      description: '上传支付截图',
      icon: Upload,
      action: () => console.log('上传支付记录'),
      color: 'bg-orange-500'
    }
  ]

  const getStatusBadge = (status) => {
    const statusConfig = {
      pending: { label: '待处理', variant: 'destructive' },
      completed: { label: '已完成', variant: 'default' },
      settled: { label: '已结算', variant: 'secondary' }
    }
    
    const config = statusConfig[status] || statusConfig.pending
    return <Badge variant={config.variant}>{config.label}</Badge>
  }

  return (
    <div className="space-y-6">
      {/* Welcome Section */}
      <div className="bg-gradient-to-r from-blue-600 to-purple-600 rounded-lg p-6 text-white">
        <h2 className="text-2xl font-bold mb-2">
          欢迎回来，{user?.nickname || user?.username || '用户'}！
        </h2>
        <p className="text-blue-100">
          今天是个记账的好日子，让我们开始管理您的拼团账本吧。
        </p>
      </div>

      {/* Stats Cards */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">拼团数量</CardTitle>
            <Users className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{stats.totalGroups}</div>
            <p className="text-xs text-muted-foreground">
              +1 较上月
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">活跃账本</CardTitle>
            <Receipt className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{stats.activeBills}</div>
            <p className="text-xs text-muted-foreground">
              +2 较上周
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">总余额</CardTitle>
            <Wallet className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">¥{stats.totalBalance.toFixed(2)}</div>
            <p className="text-xs text-muted-foreground">
              +5.2% 较上月
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">待处理交易</CardTitle>
            <TrendingUp className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{stats.pendingTransactions}</div>
            <p className="text-xs text-muted-foreground">
              需要您的处理
            </p>
          </CardContent>
        </Card>
      </div>

      {/* Quick Actions */}
      <Card>
        <CardHeader>
          <CardTitle>快速操作</CardTitle>
          <CardDescription>
            选择一个操作快速开始记账
          </CardDescription>
        </CardHeader>
        <CardContent>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
            {quickActions.map((action, index) => (
              <Button
                key={index}
                variant="outline"
                className="h-auto p-4 flex flex-col items-center space-y-2 hover:shadow-md transition-shadow"
                onClick={action.action}
              >
                <div className={`p-3 rounded-full ${action.color} text-white`}>
                  <action.icon className="h-6 w-6" />
                </div>
                <div className="text-center">
                  <div className="font-medium">{action.title}</div>
                  <div className="text-xs text-muted-foreground">{action.description}</div>
                </div>
              </Button>
            ))}
          </div>
        </CardContent>
      </Card>

      {/* Recent Activities */}
      <Card>
        <CardHeader>
          <CardTitle>最近活动</CardTitle>
          <CardDescription>
            您最近的交易和记账活动
          </CardDescription>
        </CardHeader>
        <CardContent>
          <div className="space-y-4">
            {recentActivities.map((activity) => (
              <div key={activity.id} className="flex items-center justify-between p-4 border rounded-lg">
                <div className="flex items-center space-x-4">
                  <div className={`p-2 rounded-full ${
                    activity.type === 'expense' ? 'bg-red-100 text-red-600' : 'bg-green-100 text-green-600'
                  }`}>
                    {activity.type === 'expense' ? (
                      <Receipt className="h-4 w-4" />
                    ) : (
                      <Wallet className="h-4 w-4" />
                    )}
                  </div>
                  <div>
                    <div className="font-medium">{activity.description}</div>
                    <div className="text-sm text-muted-foreground">
                      {activity.group} • {activity.date}
                    </div>
                  </div>
                </div>
                <div className="flex items-center space-x-2">
                  <div className="text-right">
                    <div className={`font-medium ${
                      activity.type === 'expense' ? 'text-red-600' : 'text-green-600'
                    }`}>
                      {activity.type === 'expense' ? '-' : '+'}¥{activity.amount.toFixed(2)}
                    </div>
                  </div>
                  {getStatusBadge(activity.status)}
                </div>
              </div>
            ))}
          </div>
          <div className="mt-4 text-center">
            <Button variant="outline">查看全部活动</Button>
          </div>
        </CardContent>
      </Card>
    </div>
  )
}

export default Dashboard

