namespace Domain.Test;

public class SanityCheck
{
    [Fact]
    public void TrueIsTrue()
    {
        Assert.True(true);
    }

    [Fact]
    public void Fail()
    {
        Assert.True(false);
    }
}
