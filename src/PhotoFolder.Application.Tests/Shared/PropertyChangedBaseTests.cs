using PhotoFolder.Application.Shared;
using Xunit;

namespace PhotoFolder.Application.Tests.Shared
{
    public class PropertyChangedBaseTests : PropertyChangedBase
    {
        private string _testProperty;

        public string TestProperty
        {
            get => _testProperty;
            set => _testProperty = value;
        }

        [Fact]
        public void TestGetPropertyName()
        {
            Assert.Equal(nameof(TestProperty), GetPropertyName(() => TestProperty));
        }

        [Fact]
        public void TestOnPropertyChanged()
        {
            var raised = false;
            PropertyChanged += (sender, args) =>
            {
                Assert.Equal(nameof(TestProperty), args.PropertyName);
                raised = true;
            };
            OnPropertyChanged(nameof(TestProperty));

            Assert.True(raised);
        }

        [Fact]
        public void TestOnPropertyChangedExpression()
        {
            var raised = false;
            PropertyChanged += (sender, args) =>
            {
                Assert.Equal(nameof(TestProperty), args.PropertyName);
                raised = true;
            };
            OnPropertyChanged(() => TestProperty);

            Assert.True(raised);
        }

        [Fact]
        public void TestSetProperty()
        {
            var raised = false;
            PropertyChanged += (sender, args) =>
            {
                Assert.Equal(nameof(TestProperty), args.PropertyName);
                raised = true;
            };

            Assert.True(SetProperty(ref _testProperty, "test", nameof(TestProperty)));
            Assert.True(raised);

            raised = false;
            Assert.False(SetProperty(ref _testProperty, "test", nameof(TestProperty)));
            Assert.False(raised);
        }
    }

}
