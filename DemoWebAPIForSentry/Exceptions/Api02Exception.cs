namespace DemoWebAPIForSentry.Exceptions
{
    [Serializable]
    public class Api02Exception : Exception
    {
        public Api02Exception() { }

        public Api02Exception(string name)
            : base(string.Format("Api02 Exception"))
        {

        }
    }
}
