"use client";

import { useMemo, useState } from "react";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { Input } from "@/components/ui/input";
import { Badge } from "@/components/ui/badge";
import { Search, Calendar, TicketPercent } from "lucide-react";
import { type Coupon, type CouponType } from "@/lib/api";
import { useCoupons } from "@/hooks/use-api";

function badgeClasses(type: CouponType) {
  switch (type) {
    case "FixedAmount":
      return "bg-blue-100 text-blue-700 border-blue-200";
    case "Percentage":
      return "bg-green-100 text-green-700 border-green-200";
    case "FreeShipping":
      return "bg-purple-100 text-purple-700 border-purple-200";
    default:
      return "";
  }
}

export function CouponsDataTable() {
  const { data, isLoading, error, refetch } = useCoupons();
  const [search, setSearch] = useState("");

  const coupons = data?.coupons ?? [];

  const filtered = useMemo(() => {
    const q = search.toLowerCase();
    return coupons.filter((c) =>
      c.couponCode.toLowerCase().includes(q)
    );
  }, [coupons, search]);

  if (isLoading) {
    return (
      <Card>
        <CardContent className="p-6">
          <div className="text-center text-muted-foreground">Loading coupons...</div>
        </CardContent>
      </Card>
    );
  }

  if (error) {
    return (
      <Card>
        <CardContent className="p-6">
          <div className="text-center">
            <div className="text-red-500 mb-3">Failed to load coupons.</div>
            <button className="text-sm underline" onClick={() => refetch()}>Try again</button>
          </div>
        </CardContent>
      </Card>
    );
  }

  return (
    <Card>
      <CardHeader>
        <CardTitle className="flex items-center gap-2">
          <TicketPercent className="h-5 w-5" />
          Coupons ({filtered.length})
        </CardTitle>
      </CardHeader>
      <CardContent>
        <div className="mb-4">
          <div className="relative">
            <Search className="h-4 w-4 absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground" />
            <Input className="pl-9" placeholder="Search by code..." value={search} onChange={(e) => setSearch(e.target.value)} />
          </div>
        </div>

        <div className="rounded-md border">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Code</TableHead>
                <TableHead>Type</TableHead>
                <TableHead>Expires</TableHead>
                <TableHead>Usages Left</TableHead>
                <TableHead>Times/User</TableHead>
                <TableHead>Discount</TableHead>
                <TableHead>Max Discount</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {filtered.length === 0 ? (
                <TableRow>
                  <TableCell colSpan={7} className="text-center py-8 text-muted-foreground">No coupons found.</TableCell>
                </TableRow>
              ) : (
                filtered.map((c) => (
                  <TableRow key={c.id}>
                    <TableCell className="font-mono">{c.couponCode}</TableCell>
                    <TableCell>
                      <Badge variant="secondary" className={badgeClasses(c.couponType)}>
                        {c.couponType}
                      </Badge>
                    </TableCell>
                    <TableCell>
                      <div className="flex items-center gap-2 text-sm text-muted-foreground">
                        <Calendar className="h-4 w-4" />
                        {new Date(c.expiresAt).toLocaleString()}
                      </div>
                    </TableCell>
                    <TableCell>{c.usagesLeft}</TableCell>
                    <TableCell>{c.timesPerUser}</TableCell>
                    <TableCell>{c.couponType === 'FreeShipping' ? '-' : c.discountValue}</TableCell>
                    <TableCell>{c.couponType === 'FreeShipping' ? '-' : c.maxDiscount}</TableCell>
                  </TableRow>
                ))
              )}
            </TableBody>
          </Table>
        </div>
      </CardContent>
    </Card>
  );
}
