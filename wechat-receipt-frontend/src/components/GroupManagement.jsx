import { useState, useEffect } from 'react'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card.jsx'
import { Button } from '@/components/ui/button.jsx'
import { Badge } from '@/components/ui/badge.jsx'
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar.jsx'
import { Input } from '@/components/ui/input.jsx'
import { Label } from '@/components/ui/label.jsx'
import { Textarea } from '@/components/ui/textarea.jsx'
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from '@/components/ui/dialog.jsx'
import { 
  Users, 
  Plus, 
  Settings, 
  UserPlus,
  Copy,
  Eye,
  MoreHorizontal
} from 'lucide-react'

const GroupManagement = () => {
  const [groups, setGroups] = useState([])
  const [isCreateDialogOpen, setIsCreateDialogOpen] = useState(false)
  const [newGroup, setNewGroup] = useState({
    groupName: '',
    description: '',
    currency: 'CNY',
    maxMembers: 50,
    isPublic: false
  })

  useEffect(() => {
    // Mock data - in real app, fetch from API
    setGroups([
      {
        groupId: '1',
        groupName: '同事聚餐群',
        description: '公司同事聚餐费用分摊',
        createdByUsername: '张三',
        createdAt: '2025-01-15',
        status: 'Active',
        currency: 'CNY',
        currentMemberCount: 8,
        maxMembers: 20,
        isPublic: false,
        inviteCode: 'ABC123',
        members: [
          { userId: '1', username: '张三', nickname: '张三', avatarUrl: '', role: 'Creator' },
          { userId: '2', username: '李四', nickname: '李四', avatarUrl: '', role: 'Admin' },
          { userId: '3', username: '王五', nickname: '王五', avatarUrl: '', role: 'Member' },
        ]
      },
      {
        groupId: '2',
        groupName: '周末出游',
        description: '周末旅游费用管理',
        createdByUsername: '李四',
        createdAt: '2025-01-10',
        status: 'Active',
        currency: 'CNY',
        currentMemberCount: 5,
        maxMembers: 10,
        isPublic: true,
        inviteCode: 'XYZ789',
        members: [
          { userId: '2', username: '李四', nickname: '李四', avatarUrl: '', role: 'Creator' },
          { userId: '1', username: '张三', nickname: '张三', avatarUrl: '', role: 'Member' },
        ]
      },
      {
        groupId: '3',
        groupName: '朋友聚会',
        description: '朋友聚会活动费用',
        createdByUsername: '王五',
        createdAt: '2025-01-05',
        status: 'Completed',
        currency: 'CNY',
        currentMemberCount: 6,
        maxMembers: 15,
        isPublic: false,
        inviteCode: 'DEF456',
        members: [
          { userId: '3', username: '王五', nickname: '王五', avatarUrl: '', role: 'Creator' },
        ]
      }
    ])
  }, [])

  const handleCreateGroup = () => {
    // In real app, send API request
    const group = {
      ...newGroup,
      groupId: Date.now().toString(),
      createdByUsername: '当前用户',
      createdAt: new Date().toISOString().split('T')[0],
      status: 'Active',
      currentMemberCount: 1,
      inviteCode: Math.random().toString(36).substring(2, 8).toUpperCase(),
      members: [
        { userId: 'current', username: '当前用户', nickname: '当前用户', avatarUrl: '', role: 'Creator' }
      ]
    }
    
    setGroups([group, ...groups])
    setIsCreateDialogOpen(false)
    setNewGroup({
      groupName: '',
      description: '',
      currency: 'CNY',
      maxMembers: 50,
      isPublic: false
    })
  }

  const copyInviteCode = (code) => {
    navigator.clipboard.writeText(code)
    // In real app, show toast notification
    alert(`邀请码 ${code} 已复制到剪贴板`)
  }

  const getStatusBadge = (status) => {
    const statusConfig = {
      Active: { label: '活跃', variant: 'default' },
      Completed: { label: '已完成', variant: 'secondary' },
      Archived: { label: '已归档', variant: 'outline' }
    }
    
    const config = statusConfig[status] || statusConfig.Active
    return <Badge variant={config.variant}>{config.label}</Badge>
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex justify-between items-center">
        <div>
          <h2 className="text-2xl font-bold">拼团管理</h2>
          <p className="text-muted-foreground">管理您的拼团和成员</p>
        </div>
        <Dialog open={isCreateDialogOpen} onOpenChange={setIsCreateDialogOpen}>
          <DialogTrigger asChild>
            <Button className="bg-blue-600 hover:bg-blue-700">
              <Plus className="h-4 w-4 mr-2" />
              创建拼团
            </Button>
          </DialogTrigger>
          <DialogContent className="sm:max-w-[425px]">
            <DialogHeader>
              <DialogTitle>创建新拼团</DialogTitle>
              <DialogDescription>
                创建一个新的拼团来管理共同费用
              </DialogDescription>
            </DialogHeader>
            <div className="grid gap-4 py-4">
              <div className="grid gap-2">
                <Label htmlFor="groupName">拼团名称</Label>
                <Input
                  id="groupName"
                  value={newGroup.groupName}
                  onChange={(e) => setNewGroup({...newGroup, groupName: e.target.value})}
                  placeholder="输入拼团名称"
                />
              </div>
              <div className="grid gap-2">
                <Label htmlFor="description">描述</Label>
                <Textarea
                  id="description"
                  value={newGroup.description}
                  onChange={(e) => setNewGroup({...newGroup, description: e.target.value})}
                  placeholder="输入拼团描述"
                />
              </div>
              <div className="grid grid-cols-2 gap-4">
                <div className="grid gap-2">
                  <Label htmlFor="currency">货币</Label>
                  <select
                    id="currency"
                    value={newGroup.currency}
                    onChange={(e) => setNewGroup({...newGroup, currency: e.target.value})}
                    className="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background"
                  >
                    <option value="CNY">人民币 (CNY)</option>
                    <option value="USD">美元 (USD)</option>
                    <option value="EUR">欧元 (EUR)</option>
                  </select>
                </div>
                <div className="grid gap-2">
                  <Label htmlFor="maxMembers">最大成员数</Label>
                  <Input
                    id="maxMembers"
                    type="number"
                    value={newGroup.maxMembers}
                    onChange={(e) => setNewGroup({...newGroup, maxMembers: parseInt(e.target.value)})}
                    min="2"
                    max="100"
                  />
                </div>
              </div>
              <div className="flex items-center space-x-2">
                <input
                  type="checkbox"
                  id="isPublic"
                  checked={newGroup.isPublic}
                  onChange={(e) => setNewGroup({...newGroup, isPublic: e.target.checked})}
                  className="rounded border-gray-300"
                />
                <Label htmlFor="isPublic">公开拼团（任何人都可以加入）</Label>
              </div>
            </div>
            <DialogFooter>
              <Button variant="outline" onClick={() => setIsCreateDialogOpen(false)}>
                取消
              </Button>
              <Button onClick={handleCreateGroup}>创建拼团</Button>
            </DialogFooter>
          </DialogContent>
        </Dialog>
      </div>

      {/* Groups Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {groups.map((group) => (
          <Card key={group.groupId} className="hover:shadow-md transition-shadow">
            <CardHeader>
              <div className="flex justify-between items-start">
                <div className="flex-1">
                  <CardTitle className="text-lg">{group.groupName}</CardTitle>
                  <CardDescription className="mt-1">
                    {group.description}
                  </CardDescription>
                </div>
                <div className="flex items-center space-x-2">
                  {getStatusBadge(group.status)}
                  <Button variant="ghost" size="sm">
                    <MoreHorizontal className="h-4 w-4" />
                  </Button>
                </div>
              </div>
            </CardHeader>
            <CardContent className="space-y-4">
              {/* Group Info */}
              <div className="grid grid-cols-2 gap-4 text-sm">
                <div>
                  <div className="text-muted-foreground">创建者</div>
                  <div className="font-medium">{group.createdByUsername}</div>
                </div>
                <div>
                  <div className="text-muted-foreground">创建时间</div>
                  <div className="font-medium">{group.createdAt}</div>
                </div>
                <div>
                  <div className="text-muted-foreground">成员数量</div>
                  <div className="font-medium">
                    {group.currentMemberCount}/{group.maxMembers}
                  </div>
                </div>
                <div>
                  <div className="text-muted-foreground">货币</div>
                  <div className="font-medium">{group.currency}</div>
                </div>
              </div>

              {/* Members Preview */}
              <div>
                <div className="text-sm text-muted-foreground mb-2">成员</div>
                <div className="flex items-center space-x-2">
                  <div className="flex -space-x-2">
                    {group.members.slice(0, 3).map((member, index) => (
                      <Avatar key={member.userId} className="h-8 w-8 border-2 border-background">
                        <AvatarImage src={member.avatarUrl} alt={member.nickname} />
                        <AvatarFallback className="text-xs">
                          {member.nickname?.charAt(0) || 'U'}
                        </AvatarFallback>
                      </Avatar>
                    ))}
                    {group.currentMemberCount > 3 && (
                      <div className="h-8 w-8 rounded-full bg-muted border-2 border-background flex items-center justify-center text-xs font-medium">
                        +{group.currentMemberCount - 3}
                      </div>
                    )}
                  </div>
                  <Button variant="ghost" size="sm">
                    <Eye className="h-4 w-4" />
                  </Button>
                </div>
              </div>

              {/* Invite Code */}
              <div className="bg-muted rounded-lg p-3">
                <div className="flex justify-between items-center">
                  <div>
                    <div className="text-sm text-muted-foreground">邀请码</div>
                    <div className="font-mono font-medium">{group.inviteCode}</div>
                  </div>
                  <Button 
                    variant="ghost" 
                    size="sm"
                    onClick={() => copyInviteCode(group.inviteCode)}
                  >
                    <Copy className="h-4 w-4" />
                  </Button>
                </div>
              </div>

              {/* Actions */}
              <div className="flex space-x-2">
                <Button variant="outline" size="sm" className="flex-1">
                  <Users className="h-4 w-4 mr-2" />
                  管理成员
                </Button>
                <Button variant="outline" size="sm" className="flex-1">
                  <Settings className="h-4 w-4 mr-2" />
                  设置
                </Button>
              </div>
            </CardContent>
          </Card>
        ))}
      </div>

      {/* Empty State */}
      {groups.length === 0 && (
        <Card className="text-center py-12">
          <CardContent>
            <Users className="h-12 w-12 text-muted-foreground mx-auto mb-4" />
            <h3 className="text-lg font-medium mb-2">还没有拼团</h3>
            <p className="text-muted-foreground mb-4">
              创建您的第一个拼团来开始管理共同费用
            </p>
            <Button onClick={() => setIsCreateDialogOpen(true)}>
              <Plus className="h-4 w-4 mr-2" />
              创建拼团
            </Button>
          </CardContent>
        </Card>
      )}
    </div>
  )
}

export default GroupManagement

