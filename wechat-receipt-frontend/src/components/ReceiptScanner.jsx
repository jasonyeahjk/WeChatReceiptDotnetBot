import { useState, useRef } from 'react'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card.jsx'
import { Button } from '@/components/ui/button.jsx'
import { Input } from '@/components/ui/input.jsx'
import { Label } from '@/components/ui/label.jsx'
import { Textarea } from '@/components/ui/textarea.jsx'
import { Badge } from '@/components/ui/badge.jsx'
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog.jsx'
import { 
  Camera, 
  Upload, 
  Scan,
  CheckCircle,
  AlertCircle,
  Loader2,
  X,
  Edit
} from 'lucide-react'

const ReceiptScanner = ({ isOpen, onClose, onSave }) => {
  const [scanStep, setScanStep] = useState('upload') // upload, processing, review, confirm
  const [selectedFile, setSelectedFile] = useState(null)
  const [previewUrl, setPreviewUrl] = useState('')
  const [isProcessing, setIsProcessing] = useState(false)
  const [recognitionResult, setRecognitionResult] = useState(null)
  const [editedData, setEditedData] = useState({})
  const fileInputRef = useRef(null)
  const cameraInputRef = useRef(null)

  const handleFileSelect = (event) => {
    const file = event.target.files[0]
    if (file) {
      setSelectedFile(file)
      const url = URL.createObjectURL(file)
      setPreviewUrl(url)
      setScanStep('review')
    }
  }

  const handleCameraCapture = () => {
    cameraInputRef.current?.click()
  }

  const handleUploadClick = () => {
    fileInputRef.current?.click()
  }

  const processReceipt = async () => {
    if (!selectedFile) return

    setIsProcessing(true)
    setScanStep('processing')

    try {
      // Convert file to base64
      const base64 = await fileToBase64(selectedFile)
      
      // Call Donut service
      const response = await fetch('http://localhost:8000/api/recognize/receipt', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          image: base64
        })
      })

      const result = await response.json()
      
      if (result.success) {
        setRecognitionResult(result.extracted_data)
        setEditedData({
          amount: result.extracted_data.extracted_amount || 0,
          description: result.extracted_data.extracted_description || '',
          merchant: result.extracted_data.merchant || '',
          items: result.extracted_data.extracted_items || [],
          date: result.extracted_data.extracted_date || new Date().toISOString().split('T')[0]
        })
        setScanStep('confirm')
      } else {
        throw new Error(result.error || '识别失败')
      }
    } catch (error) {
      console.error('Receipt processing error:', error)
      alert('收据识别失败，请重试或手动输入')
      setScanStep('review')
    } finally {
      setIsProcessing(false)
    }
  }

  const fileToBase64 = (file) => {
    return new Promise((resolve, reject) => {
      const reader = new FileReader()
      reader.readAsDataURL(file)
      reader.onload = () => resolve(reader.result.split(',')[1])
      reader.onerror = error => reject(error)
    })
  }

  const handleSave = () => {
    const transactionData = {
      ...editedData,
      imageUrl: previewUrl,
      hasImageRecognition: true,
      recognitionData: recognitionResult
    }
    
    onSave(transactionData)
    handleClose()
  }

  const handleClose = () => {
    setScanStep('upload')
    setSelectedFile(null)
    setPreviewUrl('')
    setRecognitionResult(null)
    setEditedData({})
    setIsProcessing(false)
    onClose()
  }

  const renderUploadStep = () => (
    <div className="space-y-6">
      <div className="text-center">
        <Scan className="h-12 w-12 text-muted-foreground mx-auto mb-4" />
        <h3 className="text-lg font-medium mb-2">扫描收据</h3>
        <p className="text-muted-foreground">
          选择拍照或上传收据图片，我们将自动识别其中的信息
        </p>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <Button
          variant="outline"
          className="h-24 flex flex-col items-center space-y-2"
          onClick={handleCameraCapture}
        >
          <Camera className="h-8 w-8" />
          <span>拍照扫描</span>
        </Button>

        <Button
          variant="outline"
          className="h-24 flex flex-col items-center space-y-2"
          onClick={handleUploadClick}
        >
          <Upload className="h-8 w-8" />
          <span>上传图片</span>
        </Button>
      </div>

      <input
        ref={fileInputRef}
        type="file"
        accept="image/*"
        onChange={handleFileSelect}
        className="hidden"
      />
      
      <input
        ref={cameraInputRef}
        type="file"
        accept="image/*"
        capture="environment"
        onChange={handleFileSelect}
        className="hidden"
      />
    </div>
  )

  const renderReviewStep = () => (
    <div className="space-y-6">
      <div className="text-center">
        <h3 className="text-lg font-medium mb-2">预览图片</h3>
        <p className="text-muted-foreground">
          确认图片清晰可读，然后点击识别按钮
        </p>
      </div>

      {previewUrl && (
        <div className="flex justify-center">
          <img
            src={previewUrl}
            alt="Receipt preview"
            className="max-w-full max-h-64 object-contain border rounded-lg"
          />
        </div>
      )}

      <div className="flex space-x-3">
        <Button variant="outline" onClick={() => setScanStep('upload')} className="flex-1">
          重新选择
        </Button>
        <Button onClick={processReceipt} className="flex-1">
          开始识别
        </Button>
      </div>
    </div>
  )

  const renderProcessingStep = () => (
    <div className="space-y-6 text-center">
      <Loader2 className="h-12 w-12 text-blue-600 mx-auto animate-spin" />
      <div>
        <h3 className="text-lg font-medium mb-2">正在识别收据</h3>
        <p className="text-muted-foreground">
          AI正在分析您的收据，请稍候...
        </p>
      </div>
    </div>
  )

  const renderConfirmStep = () => (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h3 className="text-lg font-medium">确认识别结果</h3>
        <Badge variant="secondary" className="flex items-center space-x-1">
          <CheckCircle className="h-3 w-3" />
          <span>置信度: {Math.round((recognitionResult?.confidence || 0) * 100)}%</span>
        </Badge>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        {/* Image Preview */}
        <div>
          <Label className="text-sm font-medium">原始图片</Label>
          <img
            src={previewUrl}
            alt="Receipt"
            className="w-full max-h-48 object-contain border rounded-lg mt-2"
          />
        </div>

        {/* Extracted Data */}
        <div className="space-y-4">
          <div>
            <Label htmlFor="amount">金额</Label>
            <Input
              id="amount"
              type="number"
              step="0.01"
              value={editedData.amount || ''}
              onChange={(e) => setEditedData({...editedData, amount: parseFloat(e.target.value)})}
              className="mt-1"
            />
          </div>

          <div>
            <Label htmlFor="description">描述</Label>
            <Input
              id="description"
              value={editedData.description || ''}
              onChange={(e) => setEditedData({...editedData, description: e.target.value})}
              className="mt-1"
            />
          </div>

          <div>
            <Label htmlFor="merchant">商家</Label>
            <Input
              id="merchant"
              value={editedData.merchant || ''}
              onChange={(e) => setEditedData({...editedData, merchant: e.target.value})}
              className="mt-1"
            />
          </div>

          <div>
            <Label htmlFor="date">日期</Label>
            <Input
              id="date"
              type="date"
              value={editedData.date || ''}
              onChange={(e) => setEditedData({...editedData, date: e.target.value})}
              className="mt-1"
            />
          </div>
        </div>
      </div>

      {/* Items List */}
      {editedData.items && editedData.items.length > 0 && (
        <div>
          <Label className="text-sm font-medium">识别的项目</Label>
          <div className="mt-2 space-y-2">
            {editedData.items.map((item, index) => (
              <div key={index} className="p-2 bg-muted rounded text-sm">
                {item}
              </div>
            ))}
          </div>
        </div>
      )}

      <div className="flex space-x-3">
        <Button variant="outline" onClick={() => setScanStep('review')} className="flex-1">
          重新识别
        </Button>
        <Button onClick={handleSave} className="flex-1">
          确认保存
        </Button>
      </div>
    </div>
  )

  return (
    <Dialog open={isOpen} onOpenChange={handleClose}>
      <DialogContent className="sm:max-w-[600px] max-h-[80vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle className="flex items-center space-x-2">
            <Scan className="h-5 w-5" />
            <span>收据扫描识别</span>
          </DialogTitle>
          <DialogDescription>
            使用AI技术自动识别收据信息，快速创建交易记录
          </DialogDescription>
        </DialogHeader>

        <div className="py-4">
          {scanStep === 'upload' && renderUploadStep()}
          {scanStep === 'review' && renderReviewStep()}
          {scanStep === 'processing' && renderProcessingStep()}
          {scanStep === 'confirm' && renderConfirmStep()}
        </div>

        {scanStep !== 'processing' && (
          <DialogFooter>
            <Button variant="outline" onClick={handleClose}>
              取消
            </Button>
          </DialogFooter>
        )}
      </DialogContent>
    </Dialog>
  )
}

export default ReceiptScanner

