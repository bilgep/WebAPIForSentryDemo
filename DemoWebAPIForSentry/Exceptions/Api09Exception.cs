namespace DemoWebAPIForSentry.Exceptions
{
    [Serializable]
    public class Api09Exception : Exception
    {
        public Api09Exception() { }

        public Api09Exception(string name)
            : base(string.Format("Api09 Exception"))
        {
             
        }
    }
}
