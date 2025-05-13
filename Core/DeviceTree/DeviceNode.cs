namespace IIOTFramework.Core
{
    public class BaseNode
    {
        // Class method.
        private static readonly Dictionary<string, BaseNode> _devices = new();
        public static BaseNode? find(string nickname) =>
            _devices.TryGetValue(nickname, out var n) ? n : null;
        // Node properties.
        public string DevID { get; private set; } = Guid.NewGuid().ToString();
        public string? NickName { get; set; }
        public BaseNode? Parent { get; private set; }
        public List<BaseNode> Children { get; } = new();
        private DevStatus _status;
        public DevStatus Status {
            get => _status;
            protected set {
                if (!Enum.IsDefined(typeof(DevStatus), value))
                    throw new ArgumentException($"Unavailable Status: {value}");

                _status = value;
                OnDeviceStatusChanged?.Invoke(this);
            }
        }
        // Node methods.
        public event Action<BaseNode>? OnDeviceStatusChanged; // Device bubbles.
        public void AddChild(BaseNode device) => Children.Add(device);
        // constructor.
        public BaseNode(BaseNode? parent = null, string? nickname = null)
        {
            DevID = Guid.NewGuid().ToString();
            NickName = nickname;
            Parent = parent;
            Parent?.AddChild(this);
            Status = DevStatus.UnInitial;
            if (NickName != null)
                _devices.Add(NickName, this);
            if (Parent != null)
                OnDeviceStatusChanged += Parent.HandleChildDeviceStatusChange;
        }
        // event bubbling
        protected virtual void HandleChildDeviceStatusChange(BaseNode dev)
        {
            // bubble to parrent.
            if (Parent is null)
                Console.WriteLine($"[Notice] Device: {dev.NickName ?? dev.DevID} Status Change to: {dev.Status}");
            OnDeviceStatusChanged?.Invoke(dev);
        }

        // Display The Tree from Node.
        public void Display(int level = 0)
        {
            Console.WriteLine($"{new string(' ', level * 2)}- {NickName} [{Status}] {DevID}");
            foreach (var device in Children)
                device.Display(level + 1);
        }
        public void Initialize() => Status = DevStatus.Initialized;
    }
    public class BaseNode<T> : BaseNode
    {
        public T? Device { get; private set; }
        public BaseNode(T? device = default, BaseNode? parent = null, string? nickname = null) : base(parent, nickname) => Device = device;
    }

    public enum DevStatus
    {
        UnInitial,
        Initialized,
        Active,
        Busy,
        Inactive,
        Offline,
        Error
    }
}