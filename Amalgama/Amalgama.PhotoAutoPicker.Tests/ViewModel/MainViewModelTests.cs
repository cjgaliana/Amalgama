using Amalgama.PhotoAutoPicker.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Amalgama.PhotoAutoPicker.Tests.ViewModel
{
    [TestClass]
    public class MainViewModelTests
    {

        private MainViewModel _viewModel;

        [TestInitialize]
        public void Initialize()
        {
            this._viewModel = new MainViewModel();
        }

        [TestMethod]
        public void TestMethod1()
        {
            Assert.IsTrue(string.IsNullOrEmpty(this._viewModel.ExcelPath));
        }
    }
}