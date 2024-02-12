namespace Bazaar.FbbInventory.Domain.Constants;

public static class ConvertUnits
{
    public static float FromCm3ToM3(float cm3)
    {
        return cm3 / 1000000f;
    }

    public static float FromM3ToCm3(float m3)
    {
        return m3 * 1000000f;
    }
}
