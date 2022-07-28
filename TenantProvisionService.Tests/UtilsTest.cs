namespace TenantProvisionService.Tests;

public class UtilsTest
{
    [Fact]
    public void TestCRDKind()
    {
        var crd = Utils.MakeCRD();
        bool result = (crd.Kind == "ClusterExecuter");
        Assert.True(result, "kind should be set as needed");
    }
}

