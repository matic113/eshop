"use client";

import { useMemo, useState } from "react";
import { cn } from "@/lib/utils";
import { toast } from "sonner";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Badge } from "@/components/ui/badge";
import { Calendar as CalendarIcon, PlusCircle, Sparkles } from "lucide-react";
import { type CouponType, type CreateCouponRequest } from "@/lib/api";
import { useCreateCoupon, useCoupons } from "@/hooks/use-api";
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover";
import { Calendar } from "@/components/ui/calendar";
import { format } from "date-fns";

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

function generateCouponCode(): string {
  const letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
  const digits = "0123456789";
  const rand = (n: number) => {
    const a = new Uint32Array(n);
    crypto.getRandomValues(a);
    return Array.from(a);
  };
  const letterPart = rand(5).map((v) => letters[v % letters.length]).join("");
  const digitPart = rand(3).map((v) => digits[v % digits.length]).join("");
  return `${letterPart}${digitPart}`;
}

export function CreateCouponForm() {
  const [couponCode, setCouponCode] = useState("");
  const [couponType, setCouponType] = useState<CouponType>("FixedAmount");
  const [selectedDate, setSelectedDate] = useState<Date | undefined>(undefined);
  const [usageTimes, setUsageTimes] = useState(100);
  const [timesPerUser, setTimesPerUser] = useState(1);
  const [discountValue, setDiscountValue] = useState(1);
  const [maxDiscount, setMaxDiscount] = useState(0);

  const createMutation = useCreateCoupon();
  const { refetch } = useCoupons();

  const canSubmit = useMemo(() => {
    if (!couponCode || !couponType || !selectedDate) return false;
    if (usageTimes <= 0 || timesPerUser <= 0) return false;
    if (couponType === "FixedAmount" && discountValue <= 0) return false;
    if (couponType === "Percentage" && (discountValue <= 0 || discountValue > 100)) return false;
    return true;
  }, [couponCode, couponType, selectedDate, usageTimes, timesPerUser, discountValue]);

  const onGenerate = () => {
    setCouponCode(generateCouponCode());
  };

  const onTypeChange = (next: CouponType) => {
    setCouponType(next);
    if (next === "FreeShipping") {
      setDiscountValue(1);
      setMaxDiscount(0);
    }
  };

  const onSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!selectedDate) return;
    // Build ISO at 00:00:00Z for the selected date
    const expirationDate = new Date(
      Date.UTC(
        selectedDate.getFullYear(),
        selectedDate.getMonth(),
        selectedDate.getDate(),
        0, 0, 0, 0
      )
    ).toISOString();

    const payload: CreateCouponRequest = {
      couponCode: couponCode.toUpperCase(),
      couponType,
      expirationDate,
      usageTimes,
      timesPerUser,
      discountValue: couponType === "FreeShipping" ? 1 : discountValue,
      maxDiscount: couponType === "FreeShipping" ? 0 : maxDiscount,
    };

    await createMutation.mutateAsync(payload);
    await refetch();

    // Show success toast
    toast.success("Coupon created successfully", {
      description: `Coupon code ${couponCode} will expire on ${format(selectedDate, "PPP")}`
    });

    // reset form
    setCouponCode("");
    setSelectedDate(undefined);
    setUsageTimes(100);
    setTimesPerUser(1);
    setDiscountValue(1);
    setMaxDiscount(0);
  };

  return (
    <Card>
      <CardHeader>
        <CardTitle className="flex items-center gap-2">
          <PlusCircle className="h-5 w-5" />
          Create Coupon
        </CardTitle>
      </CardHeader>
      <CardContent>
        <form onSubmit={onSubmit} className="grid gap-4">
          <div className="grid sm:grid-cols-3 gap-3 items-end">
            <div className="sm:col-span-2 grid gap-2">
              <Label htmlFor="couponCode">Coupon Code</Label>
              <Input
                id="couponCode"
                value={couponCode}
                onChange={(e) => setCouponCode(e.target.value.toUpperCase())}
                placeholder="ABCDE123"
                required
              />
            </div>
            <Button type="button" variant="outline" onClick={onGenerate} className="h-9 mt-6">
              <Sparkles className="h-4 w-4 mr-2" />
              Generate
            </Button>
          </div>

          <div className="grid sm:grid-cols-3 gap-3">
            <div className="grid gap-2">
              <Label htmlFor="couponType">Type</Label>
              <select
                id="couponType"
                value={couponType}
                onChange={(e) => onTypeChange(e.target.value as CouponType)}
                className="h-9 rounded-md border bg-background px-3 text-sm"
              >
                <option value="FixedAmount">Fixed amount</option>
                <option value="Percentage">Percentage</option>
                <option value="FreeShipping">Free shipping</option>
              </select>
            </div>
            <div className="grid gap-2">
              <Label>Expiration</Label>
              <Popover>
                <PopoverTrigger asChild>
                  <Button
                    variant="outline"
                    data-empty={!selectedDate}
                    className={cn(
                      "w-full justify-start text-left font-normal",
                      "data-[empty=true]:text-muted-foreground"
                    )}
                  >
                    <CalendarIcon className="mr-2 h-4 w-4" />
                    {selectedDate ? format(selectedDate, "PPP") : <span>Pick a date</span>}
                  </Button>
                </PopoverTrigger>
                <PopoverContent className="w-auto p-0" align="start">
                  <Calendar
                    mode="single"
                    selected={selectedDate}
                    onSelect={setSelectedDate}
                    initialFocus
                    disabled={(date) => date < new Date()} // Disable past dates
                  />
                </PopoverContent>
              </Popover>
            </div>
            <div className="grid gap-2">
              <Label>Preview</Label>
              <div className="h-9 flex items-center px-2 rounded-md border text-sm">
                <span className="mr-2">{couponCode || "—"}</span>
                <Badge variant="secondary" className={badgeClasses(couponType)}>
                  {couponType}
                </Badge>
              </div>
            </div>
          </div>

          <div className="grid sm:grid-cols-2 gap-3">
            <div className="grid gap-2">
              <Label htmlFor="usageTimes">Total Usages Allowed</Label>
              <Input id="usageTimes" type="number" min={1} value={usageTimes} onChange={(e) => setUsageTimes(Number(e.target.value))} />
            </div>
            <div className="grid gap-2">
              <Label htmlFor="timesPerUser">Times Per User</Label>
              <Input id="timesPerUser" type="number" min={1} value={timesPerUser} onChange={(e) => setTimesPerUser(Number(e.target.value))} />
            </div>
          </div>

          <div className="grid sm:grid-cols-2 gap-3">
            <div className="grid gap-2">
              <Label htmlFor="discountValue">{couponType === "Percentage" ? "Discount (%)" : "Discount Value"}</Label>
              <Input id="discountValue" type="number" min={0} step={couponType === "Percentage" ? 1 : 0.01} value={discountValue} onChange={(e) => setDiscountValue(Number(e.target.value))} disabled={couponType === "FreeShipping"} />
            </div>
            <div className="grid gap-2">
              <Label htmlFor="maxDiscount">Max Discount</Label>
              <Input id="maxDiscount" type="number" min={0} step={0.01} value={maxDiscount} onChange={(e) => setMaxDiscount(Number(e.target.value))} disabled={couponType === "FreeShipping"} />
            </div>
          </div>

          <div className="flex justify-end">
            <Button type="submit" disabled={!canSubmit || createMutation.isPending}>
              {createMutation.isPending ? "Creating..." : "Create Coupon"}
            </Button>
          </div>
        </form>
      </CardContent>
    </Card>
  );
}
