using NUnit.Framework;
using IIOTFramework.Core;

[TestFixture]
public class BaseNodeTests
{
    [Test]
    public void Should_Create_Node_With_Default_Properties()
    {
        var node = new BaseNode();
        Assert.NotNull(node.DevID);
        Assert.Null(node.NickName);
        Assert.Null(node.Parent);
        Assert.AreEqual(0, node.Children.Count);
        Assert.AreEqual(DevStatus.UnInitial, node.Status);
    }

    [Test]
    public void Should_Add_Child_Node()
    {
        var parent = new BaseNode();
        var child = new BaseNode(parent);

        Assert.AreEqual(1, parent.Children.Count);
        Assert.Contains(child, parent.Children);
    }

    [Test]
    public void Should_Find_Node_By_Nickname()
    {
        var node = new BaseNode(null, "DeviceA");
        var foundNode = BaseNode.find("DeviceA");

        Assert.NotNull(foundNode);
        Assert.AreEqual(node.DevID, foundNode!.DevID);
    }

    [Test]
    public void Should_Trigger_Status_Change_Event()
    {
        var parent = new BaseNode();
        var child = new BaseNode(parent);

        bool eventTriggered = false;
        parent.OnDeviceStatusChanged += _ => eventTriggered = true;

        child.Initialize();
        Assert.IsTrue(eventTriggered);
    }

    [Test]
    public void Should_Display_Node_Tree()
    {
        var root = new BaseNode(null, "Root");
        new BaseNode(root, "Child1");
        new BaseNode(root, "Child2");
        new BaseNode(BaseNode.find("Child2"), "Child3");

        root.Display();
        Assert.AreEqual(2, root.Children.Count);
    }
}