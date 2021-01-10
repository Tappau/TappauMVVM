namespace TappauMVVM.Contracts
{
    public interface IRaiseCanExecuteChanged
    {
        /// <summary>
        /// Will raise the CanExecuteCommand to refresh the state of the Command.
        /// </summary>
        void RaiseCanExecuteChanged();
    }
}