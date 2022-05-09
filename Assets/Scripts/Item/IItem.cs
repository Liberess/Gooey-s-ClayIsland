namespace Hun.Item
{
    public interface IItem
    {
        public void OnEnter();
        public void OnExit();
        public void UseItem();
    }
}