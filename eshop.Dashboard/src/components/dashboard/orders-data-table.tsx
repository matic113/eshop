'use client'

import { useMemo, useState } from 'react'
import { useOrders, useUpdateOrderStatus } from '@/hooks/use-api'
import {
  type Order,
  type OrderStatus,
  type OrderPeriod,
  type OrdersAdminParams,
} from '@/lib/api'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import { Input } from '@/components/ui/input'
import { Badge } from '@/components/ui/badge'
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger } from '@/components/ui/dropdown-menu'
import { Button } from '@/components/ui/button'
import { Calendar, ChevronLeft, ChevronRight, Search, Truck, ChevronsUpDown } from 'lucide-react'
import { toast } from 'sonner'
import { AlertDialog, AlertDialogAction, AlertDialogCancel, AlertDialogContent, AlertDialogDescription, AlertDialogFooter, AlertDialogHeader, AlertDialogTitle } from '@/components/ui/alert-dialog'
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger, DialogDescription } from '@/components/ui/dialog'

const PERIODS: { value: OrderPeriod; label: string }[] = [
  { value: '1d', label: 'Last 1 day' },
  { value: '3d', label: 'Last 3 days' },
  { value: '7d', label: 'Last 7 days' },
  { value: '30d', label: 'Last 30 days' },
]

const STATUSES: OrderStatus[] = [
  'Pending',
  'Completed',
  'Failed',
  'Processing',
  'Shipped',
  'Delivered',
  'Cancelled',
]

const UPDATEABLE_STATUSES: OrderStatus[] = [
  'Pending',
  'Processing',
  'Shipped',
  'Delivered',
  'Cancelled',
]

function statusBadgeClasses(status: OrderStatus) {
  switch (status) {
    case 'Pending':
      return 'bg-yellow-100 text-yellow-700 border-yellow-200'
    case 'Completed':
      return 'bg-green-100 text-green-700 border-green-200'
    case 'Failed':
      return 'bg-red-100 text-red-700 border-red-200'
    case 'Processing':
      return 'bg-blue-100 text-blue-700 border-blue-200'
    case 'Shipped':
      return 'bg-indigo-100 text-indigo-700 border-indigo-200'
    case 'Delivered':
      return 'bg-emerald-100 text-emerald-700 border-emerald-200'
    case 'Cancelled':
      return 'bg-gray-100 text-gray-700 border-gray-200'
  }
}

function paymentBadgeClasses(method: string) {
  switch (method) {
    case 'Paymob':
      return 'bg-orange-100 text-orange-700 border-orange-200'
    case 'CashOnDelivery':
      return 'bg-cyan-100 text-cyan-700 border-cyan-200'
    default:
      return 'bg-gray-100 text-gray-700 border-gray-200'
  }
}

function paymentLabel(method: string) {
  return method === 'CashOnDelivery' ? 'Cash' : method
}

export function OrdersDataTable() {
  const [params, setParams] = useState<OrdersAdminParams>({ period: '1d', page: 1, pageSize: 20 })
  const { data, isLoading, error, refetch } = useOrders(params)
  const updateStatus = useUpdateOrderStatus()
  const [search, setSearch] = useState('')
  const [confirmOpen, setConfirmOpen] = useState(false)
  const [pendingUpdate, setPendingUpdate] = useState<{ order: Order; status: OrderStatus } | null>(null)

  const items = data?.orders.items ?? []
  const filtered = useMemo(() => {
    const q = search.trim().toLowerCase()
    if (!q) return items
    return items.filter((o) =>
      [
        o.orderCode,
        o.customerInfo.fullName,
        o.customerInfo.email,
        o.customerInfo.phoneNumber,
        o.shippingDetails.phoneNumber,
      ]
        .filter(Boolean)
        .some((s) => String(s).toLowerCase().includes(q))
    )
  }, [items, search])

  const nextPage = () => setParams((p) => ({ ...p, page: (p.page || 1) + 1 }))
  const prevPage = () => setParams((p) => ({ ...p, page: Math.max((p.page || 1) - 1, 1) }))

  function onChangePeriod(period: OrderPeriod) {
    setParams((p) => ({ ...p, period, page: 1 }))
  }

  async function onUpdateStatus(order: Order, status: OrderStatus) {
    if (status === order.status) return
    setPendingUpdate({ order, status })
    setConfirmOpen(true)
  }

  async function confirmStatusUpdate() {
    if (!pendingUpdate) return
    try {
      await updateStatus.mutateAsync({ orderId: pendingUpdate.order.orderId, status: pendingUpdate.status })
      toast.success(`Updated ${pendingUpdate.order.orderCode} to ${pendingUpdate.status}`, { className: 'success' })
    } catch (err: any) {
      toast.error(err?.message || 'Failed to update order status', { className: 'error' })
    } finally {
      setConfirmOpen(false)
      setPendingUpdate(null)
    }
  }

  function formatOrderDate(dateString: string) {
    const d = new Date(dateString)
    const date = d.toLocaleDateString(undefined, {
      year: '2-digit',
      month: 'numeric',
      day: 'numeric',
    })
    const time = d.toLocaleTimeString(undefined, {
      hour: '2-digit',
      minute: '2-digit',
      hour12: true,
    })
    return `${date} ${time}`
  }

  if (isLoading) {
    return (
      <Card>
        <CardContent className="p-6">
          <div className="text-center text-muted-foreground">Loading orders...</div>
        </CardContent>
      </Card>
    )
  }

  if (error) {
    return (
      <Card>
        <CardContent className="p-6">
          <div className="text-center">
            <div className="text-red-500 mb-3">Failed to load orders.</div>
            <button className="text-sm underline" onClick={() => refetch()}>Try again</button>
          </div>
        </CardContent>
      </Card>
    )
  }

  return (
    <Card>
      <CardHeader>
        <CardTitle className="flex items-center gap-2">
          <Truck className="h-5 w-5" />
          Orders ({filtered.length})
        </CardTitle>
      </CardHeader>
      <CardContent>
        <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3 mb-4">
          <div className="relative">
            <Search className="h-4 w-4 absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground" />
            <Input className="pl-9" placeholder="Search code, name, email..." value={search} onChange={(e) => setSearch(e.target.value)} />
          </div>
          <div className="flex gap-2 items-center">
            <Calendar className="size-4 text-muted-foreground" />
            <div className="text-sm text-muted-foreground">Period</div>
            <DropdownMenu>
              <DropdownMenuTrigger asChild>
                <Button variant="outline" size="sm">{PERIODS.find(p => p.value === (params.period || '1d'))?.label || 'Last 1 day'}</Button>
              </DropdownMenuTrigger>
              <DropdownMenuContent align="start">
                {PERIODS.map(p => (
                  <DropdownMenuItem key={p.value} onClick={() => onChangePeriod(p.value)}>
                    {p.label}
                  </DropdownMenuItem>
                ))}
              </DropdownMenuContent>
            </DropdownMenu>
          </div>
          <div className="flex items-center justify-end gap-2">
            <Button variant="outline" size="sm" onClick={prevPage} disabled={!data?.orders.hasPreviousPage}>
              <ChevronLeft className="size-4" /> Prev
            </Button>
            <div className="text-sm text-muted-foreground">Page {data?.orders.page}</div>
            <Button variant="outline" size="sm" onClick={nextPage} disabled={!data?.orders.hasNextPage}>
              Next <ChevronRight className="size-4" />
            </Button>
          </div>
        </div>

        <div className="rounded-md border">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Order</TableHead>
                <TableHead>Status</TableHead>
                <TableHead>Payment</TableHead>
                <TableHead>Total</TableHead>
                <TableHead>Customer</TableHead>
                <TableHead>Shipping</TableHead>
                <TableHead>Created</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {filtered.map((order) => (
                <TableRow key={order.orderId}>
                  <TableCell className="font-medium">
                    <div className="space-y-1">
                      <div>#{order.orderCode?.slice(-6)}</div>
                    </div>
                  </TableCell>
                  <TableCell>
                    <DropdownMenu>
                      <DropdownMenuTrigger asChild>
                        <Badge className={statusBadgeClasses(order.status)}>
                          <span>{order.status}</span>
                          <ChevronsUpDown className="ml-1 size-3 opacity-70" />
                        </Badge>
                      </DropdownMenuTrigger>
                      <DropdownMenuContent align="start">
                        {UPDATEABLE_STATUSES.map((s) => (
                          <DropdownMenuItem key={s} onClick={() => onUpdateStatus(order, s)}>
                            Mark as {s}
                          </DropdownMenuItem>
                        ))}
                      </DropdownMenuContent>
                    </DropdownMenu>
                  </TableCell>
                  <TableCell>
                    <Badge className={paymentBadgeClasses(order.paymentMethod)}>
                      {paymentLabel(order.paymentMethod)}
                    </Badge>
                  </TableCell>
                  <TableCell>
                    <div className="font-medium">EGP {order.totalPrice}</div>
                    <div className="text-xs text-muted-foreground space-x-2">
                      <span>Shipping: EGP {order.shippingPrice}</span>
                      <span>
                        Coupon: {order.couponCode ? (
                          <>
                            <span className="font-medium">{order.couponCode}</span>
                            <span className="text-red-600 ml-1">(−EGP {order.discountAmout})</span>
                          </>
                        ) : (
                          'None'
                        )}
                      </span>
                    </div>
                  </TableCell>
                  <TableCell>
                    <div className="space-y-1">
                      <div className="font-medium">{order.customerInfo.fullName}</div>
                      <div className="text-xs text-muted-foreground">{order.customerInfo.email}</div>
                    </div>
                  </TableCell>
                  <TableCell>
                    <div className="text-xs text-muted-foreground">
                      {order.shippingDetails.state}, {order.shippingDetails.city}
                    </div>
                    <div className="text-xs font-semibold">{order.shippingDetails.phoneNumber}</div>
                    <Dialog>
                      <DialogTrigger asChild>
                        <button className="text-xs font-semibold underline underline-offset-2 text-sky-600 hover:text-sky-700">
                          View details
                        </button>
                      </DialogTrigger>
                      <DialogContent>
                        <DialogHeader>
                          <DialogTitle>Shipping details</DialogTitle>
                          <DialogDescription>Order {order.orderCode}</DialogDescription>
                        </DialogHeader>
                        <div className="space-y-3 text-sm">
                          <div>
                            <div className="text-muted-foreground">Recipient</div>
                            <div className="font-medium">{order.customerInfo.fullName}</div>
                          </div>
                          <div>
                            <div className="text-muted-foreground">Phone</div>
                            <div className="font-medium">{order.shippingDetails.phoneNumber}</div>
                          </div>
                          <div>
                            <div className="text-muted-foreground">Address</div>
                            <div className="font-medium">
                              {order.shippingDetails.street}, {order.shippingDetails.apartment}
                            </div>
                            <div className="font-medium">
                              {order.shippingDetails.city}, {order.shippingDetails.state}
                            </div>
                          </div>
                          {order.shippingDetails.notes && (
                            <div>
                              <div className="text-muted-foreground">Notes</div>
                              <div className="font-medium break-words">{order.shippingDetails.notes}</div>
                            </div>
                          )}
                        </div>
                      </DialogContent>
                    </Dialog>
                  </TableCell>
                  <TableCell>{formatOrderDate(order.createdAt)}</TableCell>
                </TableRow>
              ))}
              {filtered.length === 0 && (
                <TableRow>
                  <TableCell colSpan={7} className="text-center text-muted-foreground py-8">
                    No orders found for current filters.
                  </TableCell>
                </TableRow>
              )}
            </TableBody>
          </Table>
        </div>
      </CardContent>
      <AlertDialog open={confirmOpen} onOpenChange={setConfirmOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Change order status?</AlertDialogTitle>
            <AlertDialogDescription>
              {pendingUpdate ? `Order ${pendingUpdate.order.orderCode}: change status to ${pendingUpdate.status}?` : ''}
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel onClick={() => setConfirmOpen(false)}>Cancel</AlertDialogCancel>
            <AlertDialogAction onClick={confirmStatusUpdate} disabled={updateStatus.isPending}>
              {updateStatus.isPending ? 'Updating...' : 'Confirm'}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </Card>
  )
}


