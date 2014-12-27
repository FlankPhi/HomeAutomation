namespace HomeAutomation.Etc.Delegates
{
    public delegate void Trigger();
    public delegate bool Predicate(object obj);
    public delegate int Comparer(object obj1, object obj2);
    public delegate void Update(object sender, object value);
    public delegate void Command(byte value);
}