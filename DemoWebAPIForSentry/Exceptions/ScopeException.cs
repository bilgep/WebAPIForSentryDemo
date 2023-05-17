namespace DemoWebAPIForSentry.Exceptions
{
    [Serializable]
    public class ScopeException : Exception
    {
        public ScopeException() { }

        public ScopeException(string name)
            : base(string.Format("Scope Exception"))
        {

        }
    }
}
