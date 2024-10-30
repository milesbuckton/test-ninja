using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using TestNinja.Mocking;

namespace TestNinja.UnitTests.Mocking
{
    [TestFixture]
    public class HousekeeperServiceTests
    {
        private Mock<IEmailSender> _emailSender;
        private Housekeeper _houseKeeper;
        private Mock<IXtraMessageBox> _messageBox;
        private HousekeeperService _service;
        private Mock<IStatementGenerator> _statementGenerator;
        private Mock<IUnitOfWork> _unitOfWork;

        private readonly DateTime _statementDate;
        private readonly string _statementFileName;

        public HousekeeperServiceTests()
        {
            _statementDate = new DateTime(2017, 1, 1);
            _statementFileName = "filename";
        }

        [SetUp]
        public void SetUp()
        {
            _houseKeeper = new Housekeeper { Email = "a", FullName = "b", Oid = 1, StatementEmailBody = "c" };

            _unitOfWork = new Mock<IUnitOfWork>();
            _unitOfWork.Setup(uow => uow.Query<Housekeeper>()).Returns(new List<Housekeeper> { _houseKeeper }.AsQueryable());

            _statementGenerator = new Mock<IStatementGenerator>();
            _statementGenerator.Setup(s => s.SaveStatement(_houseKeeper.Oid, _houseKeeper.FullName, _statementDate)).Returns(_statementFileName);

            _emailSender = new Mock<IEmailSender>();
            _emailSender.Setup(e => e.EmailFile(_houseKeeper.Email, _houseKeeper.StatementEmailBody, _statementFileName, It.IsAny<string>()));

            _messageBox = new Mock<IXtraMessageBox>();
            _messageBox.Setup(m => m.Show(It.IsAny<string>(), It.IsAny<string>(), MessageBoxButtons.OK));

            _service = new HousekeeperService(_unitOfWork.Object, _statementGenerator.Object, _emailSender.Object, _messageBox.Object);
        }

        [Test]
        public void SendStatementEmails_WhenCalled_GenerateStatements()
        {
            _service.SendStatementEmails(_statementDate);

            _statementGenerator.Verify(sg => sg.SaveStatement(_houseKeeper.Oid, _houseKeeper.FullName, _statementDate));
        }

        [Test]
        [TestCase(null)]
        [TestCase(" ")]
        [TestCase("")]
        public void SendStatementEmails_HouseKeepersEmailIsNullOrWhiteSpaceOrEmpty_ShouldNotGenerateStatement(string email)
        {
            _houseKeeper.Email = email;

            _service.SendStatementEmails(_statementDate);

            _statementGenerator.Verify(s => s.SaveStatement(_houseKeeper.Oid, _houseKeeper.FullName, _statementDate), Times.Never);
        }

        [Test]
        public void SendStatementEmails_WhenCalled_EmailTheStatement()
        {
            _service.SendStatementEmails(_statementDate);

            _emailSender.Verify(s => s.EmailFile(_houseKeeper.Email, _houseKeeper.StatementEmailBody, _statementFileName, It.IsAny<string>()));
        }

        [Test]
        [TestCase(null)]
        [TestCase(" ")]
        [TestCase("")]
        public void SendStatementEmails_StatementFilenameIsNullOrWhiteSpaceOrEmpty_ShouldNotEmailTheStatement(string statementFilename)
        {
            _statementGenerator.Setup(s => s.SaveStatement(_houseKeeper.Oid, _houseKeeper.FullName, _statementDate)).Returns(statementFilename);

            _service.SendStatementEmails(_statementDate);

            _emailSender.Verify(s => s.EmailFile(_houseKeeper.Email, _houseKeeper.StatementEmailBody, _statementFileName, It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void SendStatementEmails_EmailSendingFails_DisplayAMessageBox()
        {
            _emailSender.Setup(e => e.EmailFile(_houseKeeper.Email, _houseKeeper.StatementEmailBody, _statementFileName, It.IsAny<string>())).Throws<Exception>();

            _service.SendStatementEmails(_statementDate);

            _messageBox.Verify(m => m.Show(It.IsAny<string>(), It.IsAny<string>(), MessageBoxButtons.OK));
        }
    }
}
