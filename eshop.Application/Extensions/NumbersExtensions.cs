namespace eshop.Application.Extensions
{
    public static class NumbersExtensions
    {
        public static decimal RoundMoney(this decimal value)
        {
            return Math.Round(value, 2, MidpointRounding.AwayFromZero);
        }
    }
}
