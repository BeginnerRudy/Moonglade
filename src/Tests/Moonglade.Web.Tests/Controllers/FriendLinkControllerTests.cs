using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moonglade.FriendLink;
using Moonglade.Web.Controllers;
using Moonglade.Web.Models;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Moonglade.Web.Tests.Controllers
{
    [TestFixture]
    public class FriendLinkControllerTests
    {
        private MockRepository _mockRepository;

        private Mock<IFriendLinkService> _mockFriendLinkService;
        private Mock<IMediator> _mockMediator;

        private FriendLinkEditModel _friendlinkEditViewModel = new()
        {
            LinkUrl = FakeData.Url1,
            Title = "996 ICU"
        };

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new(MockBehavior.Default);

            _mockFriendLinkService = _mockRepository.Create<IFriendLinkService>();
            _mockMediator = _mockRepository.Create<IMediator>();
        }

        private FriendLinkController CreateFriendLinkController()
        {
            return new(_mockFriendLinkService.Object, _mockMediator.Object);
        }

        [Test]
        public async Task Create_OK()
        {
            var ctl = CreateFriendLinkController();

            var result = await ctl.Create(_friendlinkEditViewModel);

            Assert.IsInstanceOf<CreatedResult>(result);
            _mockFriendLinkService.Verify(p => p.AddAsync(It.IsAny<string>(), It.IsAny<string>()));
        }

        [Test]
        public async Task Get_LinkNull()
        {
            _mockMediator.Setup(p => p.Send(new GetLinkQuery(Guid.Empty), default)).Returns(Task.FromResult((Link)null));
            var ctl = CreateFriendLinkController();
            var result = await ctl.Get(Guid.Empty);

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Get_OK()
        {
            _mockMediator.Setup(p => p.Send(It.IsAny<GetLinkQuery>(), default)).Returns(Task.FromResult(new Link()));
            var ctl = CreateFriendLinkController();
            var result = await ctl.Get(FakeData.Uid1);

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task Update_OK()
        {
            var ctl = CreateFriendLinkController();

            var result = await ctl.Update(FakeData.Uid1, _friendlinkEditViewModel);

            Assert.IsInstanceOf<NoContentResult>(result);
            _mockFriendLinkService.Verify(p => p.UpdateAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()));
        }

        [Test]
        public async Task Delete_OK()
        {
            var ctl = CreateFriendLinkController();
            var result = await ctl.Delete(FakeData.Uid1);

            Assert.IsInstanceOf<NoContentResult>(result);
            _mockMediator.Verify(p => p.Send(It.IsAny<DeleteLinkCommand>(), default));
        }
    }
}
