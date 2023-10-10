[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace ContractingTests.ControllerTests;

public class ContractControllerTests
{
    [Fact]
    public void GetById_ReturnsContract_WhenFound()
    {
        // arrange
        (var contractCtrl, var mockContractMgr) = GetSutAndMockGetByIdSetup(true);
        int id = ValidFpContract.Id;

        // act
        var result = contractCtrl.GetById(id);

        // assert
        Assert.NotNull(result.Value);

        var contract = result.Value;
        Assert.Equal(id, contract.Id);
        Assert.Equal(ValidFpContract.PartnerId, contract.PartnerId);
        Assert.Equal(ValidFpContract.SellingPlanId, contract.SellingPlanId);
        Assert.Equal(ValidFpContract.StartDate, contract.StartDate);
        Assert.Equal(ValidFpContract.EndDate, contract.EndDate);

        mockContractMgr.Verify();
    }

    [Fact]
    public void GetById_ReturnsNotFoundResult_WhenNotFound()
    {
        // arrange
        (var contractCtrl, var mockContractMgr) = GetSutAndMockGetByIdSetup(false);
        int id = ValidFpContract.Id;

        // act
        var result = contractCtrl.GetById(id);

        // assert
        Assert.IsType<NotFoundResult>(result.Result);
        mockContractMgr.Verify();
    }

    [Fact]
    public void SignPartnerForFpContract_ReturnsCreated_IfSuccess()
    {
        // arrange
        (var contractCtrl, var mockContractMgr) = GetSutAndMockSignFpSetup(
            ISignFixedPeriodContractResult.Success(ValidFpContract));

        var partnerId = ValidFpContract.PartnerId;
        (var partnerExternalId, var planId, var endDate) = GetValidFpContractInfo();

        var command = new FixedPeriodContractCreateCommand()
        {
            SellingPlanId = planId,
            EndDate = endDate
        };

        // act
        var result = contractCtrl.SignPartnerForFixedPeriodContract(
            partnerExternalId, command);

        // assert
        var objectResult = result.Result as ObjectResult;
        Assert.Equal(201, objectResult.StatusCode);

        var contract = (ContractQuery)objectResult.Value!;
        Assert.Equal(ValidFpContract.Id, contract.Id);
        Assert.Equal(partnerId, contract.PartnerId);
        Assert.Equal(planId, contract.SellingPlanId);
        Assert.Equal(DateTime.Now.Date, contract.StartDate);
        Assert.Equal(endDate, contract.EndDate);

        mockContractMgr.Verify();
    }

    [Fact]
    public void SignPartnerForFpContract_ReturnsBadRequestObjectResult_IfEndDateBeforeCurrentDate()
    {
        // arrange
        (var contractCtrl, var mockContractMgr) = GetSutAndMockSignFpSetup(
            ISignFixedPeriodContractResult.ContractEndDateBeforeCurrentDate);

        (var partnerExternalId, var planId, _) = GetValidFpContractInfo();
        var endDate = DateTime.Now.Date - TimeSpan.FromDays(1);

        var command = new FixedPeriodContractCreateCommand()
        {
            SellingPlanId = planId,
            EndDate = endDate
        };

        // act
        var result = contractCtrl.SignPartnerForFixedPeriodContract(
            partnerExternalId, command);

        // assert
        AssertResultAndProperty<BadRequestObjectResult, DateTime>(
            result, EndDatePropName, endDate);
        mockContractMgr.Verify();
    }

    [Fact]
    public void SignPartnerForFpContract_ReturnsNotFoundObjectResult_IfNoPartnerFound()
    {
        // arrange
        (var contractCtrl, var mockContractMgr) = GetSutAndMockSignFpSetup(
            ISignFixedPeriodContractResult.PartnerNotFoundError);

        var partnerExternalId = "PNER-3";
        (_, var planId, var endDate) = GetValidFpContractInfo();

        var command = new FixedPeriodContractCreateCommand()
        {
            SellingPlanId = planId,
            EndDate = endDate
        };

        // act
        var result = contractCtrl.SignPartnerForFixedPeriodContract(
            partnerExternalId, command);

        // assert
        AssertResultAndProperty<NotFoundObjectResult, string>(
            result, PartnerExternalIdPropName, partnerExternalId);
        mockContractMgr.Verify();
    }

    [Fact]
    public void SignPartnerForFpContract_ReturnsNotFoundObjectResult_IfNoSellingPlanFound()
    {
        // arrange
        (var contractCtrl, var mockContractMgr) = GetSutAndMockSignFpSetup(
            ISignFixedPeriodContractResult.SellingPlanNotFoundError);

        var planId = 2;
        (var partnerExternalId, _, var endDate) = GetValidFpContractInfo();

        var command = new FixedPeriodContractCreateCommand()
        {
            SellingPlanId = planId,
            EndDate = endDate
        };

        // act
        var result = contractCtrl.SignPartnerForFixedPeriodContract(
            partnerExternalId, command);

        // assert
        AssertResultAndProperty<NotFoundObjectResult, int>(
            result, PlanIdPropName, planId);
        mockContractMgr.Verify();
    }

    [Fact]
    public void SignPartnerForFpContract_ReturnsConflictObjectResult_IfPartnerUnderContract()
    {
        // arrange
        var currentContract = ValidFpContract;
        (var contractCtrl, var mockContractMgr) = GetSutAndMockSignFpSetup(
            ISignFixedPeriodContractResult.PartnerUnderContractError(currentContract));

        var partnerExternalId = "PNER-1";
        var planId = ValidFpContract.SellingPlanId;
        var endDate = ValidFpContract.EndDate.Value;

        var command = new FixedPeriodContractCreateCommand()
        {
            SellingPlanId = planId,
            EndDate = endDate
        };

        // act
        var result = contractCtrl.SignPartnerForFixedPeriodContract(
            partnerExternalId, command);

        // assert
        AssertResultAndProperty<ConflictObjectResult, Contract>(
            result, ContractPropName, currentContract);
        mockContractMgr.Verify();
    }

    [Fact]
    public void SignPartnerForIndefContract_ReturnsCreated_IfSuccess()
    {
        // arrange
        (var contractCtrl, var mockContractMgr) = GetSutAndMockSignIndefSetup(
            ISignFixedPeriodContractResult.Success(ValidIndefContract));

        var partnerId = ValidFpContract.PartnerId;
        (var partnerExternalId, var planId) = GetValidIndefContractInfo();

        var command = new IndefiniteContractCreateCommand()
        {
            SellingPlanId = planId,
        };

        // act
        var result = contractCtrl.SignPartnerForIndefiniteContract(
            partnerExternalId, command);

        // assert
        var objectResult = result.Result as ObjectResult;
        Assert.Equal(201, objectResult.StatusCode);

        var contract = (ContractQuery)objectResult.Value!;
        Assert.Equal(ValidIndefContract.Id, contract.Id);
        Assert.Equal(partnerId, contract.PartnerId);
        Assert.Equal(planId, contract.SellingPlanId);
        Assert.Equal(DateTime.Now.Date, contract.StartDate);
        Assert.Null(contract.EndDate);

        mockContractMgr.Verify();
    }

    [Fact]
    public void SignPartnerForIndefContract_ReturnsNotFoundObjectResult_IfPartnerNotFound()
    {
        // arrange
        (var contractCtrl, var mockContractMgr) = GetSutAndMockSignIndefSetup(
            ISignIndefiniteContractResult.PartnerNotFoundError);

        var partnerExternalId = "PNER-3";
        (_, var planId) = GetValidIndefContractInfo();

        var command = new IndefiniteContractCreateCommand()
        {
            SellingPlanId = planId,
        };

        // act
        var result = contractCtrl.SignPartnerForIndefiniteContract(
            partnerExternalId, command);

        // assert
        AssertResultAndProperty<NotFoundObjectResult, string>(
            result, PartnerExternalIdPropName, partnerExternalId);
        mockContractMgr.Verify();
    }

    [Fact]
    public void SignPartnerForIndefContract_ReturnsNotFoundObjectResult_IfSellingPlanNotFound()
    {
        // arrange
        (var contractCtrl, var mockContractMgr) = GetSutAndMockSignIndefSetup(
            ISignIndefiniteContractResult.SellingPlanNotFoundError);

        var planId = 2;
        (var partnerExternalId, _) = GetValidIndefContractInfo();

        var command = new IndefiniteContractCreateCommand()
        {
            SellingPlanId = planId,
        };

        // act
        var result = contractCtrl.SignPartnerForIndefiniteContract(
            partnerExternalId, command);

        // assert
        AssertResultAndProperty<NotFoundObjectResult, int>(
            result, PlanIdPropName, planId);
        mockContractMgr.Verify();
    }

    [Fact]
    public void SignPartnerForIndefContract_ReturnsConflictObjectResult_IfPartnerUnderContract()
    {
        // arrange
        var currentContract = ValidFpContract;
        (var contractCtrl, var mockContractMgr) = GetSutAndMockSignIndefSetup(
            ISignFixedPeriodContractResult.PartnerUnderContractError(currentContract));

        (var partnerExternalId, var planId) = GetValidIndefContractInfo();

        var command = new IndefiniteContractCreateCommand()
        {
            SellingPlanId = planId,
        };

        // act
        var result = contractCtrl.SignPartnerForIndefiniteContract(
            partnerExternalId, command);

        // assert
        AssertResultAndProperty<ConflictObjectResult, Contract>(
            result, ContractPropName, currentContract);
        mockContractMgr.Verify();
    }

    [Fact]
    public void EndCurrentIndefiniteContract_ReturnsContract_WhenValid()
    {
        // arrange
        (var controller, var mockMgr) = GetSutAndMockEndIndefSetup(
            IEndContractResult.Success(ValidIndefContract));

        (var partnerExternalId, _) = GetValidIndefContractInfo();
        var contractId = ValidIndefContract.Id;
        var command = new IndefiniteContractEndCommand(true);

        // act
        var result = controller.EndCurrentIndefiniteContract(
            partnerExternalId, command);

        // assert
        Assert.NotNull(result.Value);
        Assert.Equal(contractId, result.Value.Id);
        Assert.Equal(DateTime.Now.Date, result.Value.EndDate);
        mockMgr.Verify();
    }

    [Fact]
    public void EndCurrentIndefiniteContract_ReturnsNoContentResult_WhenCommandEndedNotTrue()
    {
        // arrange
        (var controller, var mockMgr) = GetSutAndMockEndIndefSetup(
            IEndContractResult.Success(ValidIndefContract));

        (var partnerExternalId, _) = GetValidIndefContractInfo();
        var command = new IndefiniteContractEndCommand(false);

        // act
        var result = controller.EndCurrentIndefiniteContract(partnerExternalId, command);

        // assert
        Assert.IsType<NoContentResult>(result.Result);
        mockMgr.Verify(x =>
            x.EndCurrentIndefiniteContractWithPartner(partnerExternalId), Times.Never);
    }

    [Fact]
    public void EndCurrentIndefiniteContract_ReturnsNotFoundObjectResult_WhenPartnerNotFound()
    {
        // arrange
        (var controller, var mockMgr) = GetSutAndMockEndIndefSetup(
            IEndContractResult.PartnerNotFoundError);

        (var partnerExternalId, _) = GetValidIndefContractInfo();
        var command = new IndefiniteContractEndCommand(true);

        // act
        var result = controller.EndCurrentIndefiniteContract(partnerExternalId, command);

        // assert
        AssertResultAndProperty<NotFoundObjectResult, string>(
            result, PartnerExternalIdPropName, partnerExternalId);
        mockMgr.Verify();
    }

    [Fact]
    public void EndCurrentIndefiniteContract_ReturnsConflict_WhenPartnerHasNoContract()
    {
        // arrange
        (var controller, var mockMgr) = GetSutAndMockEndIndefSetup(
            IEndContractResult.ContractNotFoundError);

        (var partnerExternalId, _) = GetValidIndefContractInfo();
        var command = new IndefiniteContractEndCommand(true);

        // act
        var result = controller.EndCurrentIndefiniteContract(
            partnerExternalId, command);

        // assert
        AssertNoContracts(result);
        mockMgr.Verify();
    }

    [Fact]
    public void EndCurrentIndefiniteContract_ReturnsConflictObjectResult_WhenContractNotIndefinite()
    {
        // arrange
        (var controller, var mockMgr) = GetSutAndMockEndIndefSetup(
            IEndContractResult.ContractNotIndefiniteError);

        (var partnerExternalId, _) = GetValidIndefContractInfo();
        var command = new IndefiniteContractEndCommand(true);

        // act
        var result = controller.EndCurrentIndefiniteContract(partnerExternalId, command);

        // assert
        AssertResultAndProperty<ConflictObjectResult, string>(result, ErrorPropName);
        mockMgr.Verify();
    }

    [Fact]
    public void ExtendFixedPeriodContract_ReturnsContractQuery_WhenValid()
    {
        // arrange
        (var controller, var mockMgr) = GetSutAndMockExtendFpSetup(
            IExtendContractResult.Success(ValidFpContract));

        (var partnerExternalId, _, var endDate) = GetValidFpContractInfo();
        var contractId = ValidFpContract.Id;
        endDate += TimeSpan.FromDays(30);

        var extension = new ContractExtension
        {
            ExtendedEndDate = endDate
        };

        // act
        var result = controller.ExtendCurrentFixedPeriodContract(
            partnerExternalId, extension);

        // assert
        Assert.NotNull(result.Value);
        Assert.Equal(contractId, result.Value.Id);
        Assert.Equal(endDate, result.Value.EndDate);
        mockMgr.Verify();
    }

    [Fact]
    public void ExtendFixedPeriodContract_ReturnsNotFoundObjectResult_WhenPartnerNotFound()
    {
        // arrange
        (var controller, var mockMgr) = GetSutAndMockExtendFpSetup(
            IExtendContractResult.PartnerNotFoundError);

        (var partnerExternalId, _, var endDate) = GetValidFpContractInfo();
        endDate += TimeSpan.FromDays(30);

        var extension = new ContractExtension
        {
            ExtendedEndDate = endDate
        };

        // act
        var result = controller.ExtendCurrentFixedPeriodContract(
            partnerExternalId, extension);

        // assert
        AssertResultAndProperty<NotFoundObjectResult, string>(
            result, PartnerExternalIdPropName, partnerExternalId);
        mockMgr.Verify();
    }

    [Fact]
    public void ExtendFixedPeriodContract_ReturnsConflictObjectResult_WhenPartnerHasNoContract()
    {
        // arrange
        (var controller, var mockMgr) = GetSutAndMockExtendFpSetup(
            IExtendContractResult.ContractNotFoundError);

        (var partnerExternalId, _, var endDate) = GetValidFpContractInfo();
        endDate += TimeSpan.FromDays(30);

        var extension = new ContractExtension
        {
            ExtendedEndDate = endDate
        };

        // act
        var result = controller.ExtendCurrentFixedPeriodContract(
            partnerExternalId, extension);

        // assert
        AssertNoContracts(result);
        mockMgr.Verify();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(0)]
    public void ExtendFixedPeriodContract_ReturnsBadRequestObjectResult_WhenEndDateNotAfterOldEndDate(
        int daysBeforeNow)
    {
        // arrange
        (var controller, var mockMgr) = GetSutAndMockExtendFpSetup(
            IExtendContractResult.EndDateNotAfterOldEndDateError);

        (var partnerExternalId, _, var endDate) = GetValidFpContractInfo();
        endDate -= TimeSpan.FromDays(daysBeforeNow);

        var extension = new ContractExtension
        {
            ExtendedEndDate = endDate
        };

        // act
        var result = controller.ExtendCurrentFixedPeriodContract(
            partnerExternalId, extension);

        // assert
        AssertResultAndProperty<BadRequestObjectResult, DateTime>(
            result, ExtendedEndDatePropName, endDate);
        mockMgr.Verify();
    }

    [Fact]
    public void ExtendFixedPeriodContract_ReturnsConflictObjectResult_WhenContractNotFixedPeriod()
    {
        // arrange
        (var controller, var mockMgr) = GetSutAndMockExtendFpSetup(
            IExtendContractResult.ContractNotFixedPeriodError);

        (var partnerExternalId, _, var endDate) = GetValidFpContractInfo();
        endDate += TimeSpan.FromDays(30);

        var extension = new ContractExtension
        {
            ExtendedEndDate = endDate
        };

        // act
        var result = controller.ExtendCurrentFixedPeriodContract(partnerExternalId, extension);

        // assert
        AssertResultAndProperty<ConflictObjectResult, string>(result, ErrorPropName);
        mockMgr.Verify();
    }

    #region Constants and helpers

    private static readonly Contract ValidFpContract = ContractExtensionsAndHelpers.ValidFpContract;
    private static readonly Contract ValidIndefContract = ContractExtensionsAndHelpers.ValidIndefContract;

    private const string PartnerExternalIdPropName = "partnerExternalId";
    private const string PlanIdPropName = "sellingPlanId";
    private const string ContractPropName = "contract";
    private const string ErrorPropName = "error";
    private const string ExtendedEndDatePropName = "extendedEndDate";
    private const string EndDatePropName = "endDate";

    private static (
        string partnerExternalId, int planId, DateTime endDate) GetValidFpContractInfo()
    {
        return ("PNER-1", ValidFpContract.SellingPlanId, ValidFpContract.EndDate!.Value);
    }

    private static (string partnerExternalId, int planId) GetValidIndefContractInfo()
    {
        return ("PNER-1", ValidFpContract.SellingPlanId);
    }

    private static (ContractController, Mock<IContractManager>) GetSutAndMocks()
    {
        var mockContractMgr = new Mock<IContractManager>();
        return (new ContractController(mockContractMgr.Object), mockContractMgr);
    }

    private static (
        ContractController,
        Mock<IContractManager>) GetSutAndMockSignFpSetup(
            ISignFixedPeriodContractResult result)
    {
        (var controller, var mock) = GetSutAndMocks();

        mock.Setup(x => x
                .SignPartnerForFixedPeriod(
                    It.IsAny<string>(), It.IsAny<int>(), It.IsAny<DateTime>()))
            .Returns(() =>
            {
                if (result is ContractSuccessResult successResult)
                {
                    var contract = successResult.Contract.Clone();
                    contract.StartDate = DateTime.Now.Date;
                    successResult.Contract = contract;
                }
                return result;
            })
            .Verifiable();

        return (controller, mock);
    }

    private static (
        ContractController,
        Mock<IContractManager>) GetSutAndMockSignIndefSetup(
            ISignIndefiniteContractResult result)
    {
        (var controller, var mock) = GetSutAndMocks();

        mock.Setup(x => x
                .SignPartnerIndefinitely(
                    It.IsAny<string>(), It.IsAny<int>()))
            .Returns(() =>
            {
                if (result is ContractSuccessResult successResult)
                {
                    var contract = successResult.Contract.Clone();
                    contract.StartDate = DateTime.Now.Date;
                    successResult.Contract = contract;
                }
                return result;
            })
            .Verifiable();

        return (controller, mock);
    }

    private static (
        ContractController,
        Mock<IContractManager>) GetSutAndMockGetByIdSetup(bool found)
    {
        (var controller, var mock) = GetSutAndMocks();

        mock.Setup(x => x.GetById(It.IsAny<int>()))
            .Returns(found ? ValidFpContract : null)
            .Verifiable();

        return (controller, mock);
    }

    private static (
        ContractController,
        Mock<IContractManager>) GetSutAndMockEndIndefSetup(
        IEndContractResult result)
    {
        (var controller, var mock) = GetSutAndMocks();

        mock.Setup(x => x.EndCurrentIndefiniteContractWithPartner(It.IsAny<string>()))
            .Returns(() =>
            {
                if (result is ContractSuccessResult r)
                {
                    var contract = r.Contract.Clone();
                    contract.EndDate = DateTime.Now.Date;
                    r.Contract = contract;
                }
                return result;
            })
            .Verifiable();

        return (controller, mock);
    }

    private static (
        ContractController,
        Mock<IContractManager>) GetSutAndMockExtendFpSetup(
        IExtendContractResult result)
    {
        (var controller, var mock) = GetSutAndMocks();

        mock.Setup(x => x.ExtendCurrentFixedPeriodContractWithPartner(
                It.IsAny<string>(), It.IsAny<DateTime>()))
            .Returns<string, DateTime>((_, endDate) =>
            {
                if (result is ContractSuccessResult r)
                {
                    var contract = r.Contract.Clone();
                    contract.EndDate = endDate;
                    r.Contract = contract;
                }
                return result;
            })
            .Verifiable();

        return (controller, mock);
    }

    private static void AssertNoContracts(ActionResult<ContractQuery> result)
    {
        var notFoundResult = Assert.IsType<ConflictObjectResult>(result.Result);
        Assert.NotNull(notFoundResult.Value);

        var contracts = notFoundResult.Value
            .GetType()
            .GetProperty("contracts", typeof(Contract[]))?
            .GetValue(notFoundResult.Value) as Contract[];

        Assert.NotNull(contracts);
        Assert.Empty(contracts);
    }

    private static void AssertResultAndProperty<TResultType, TPropType>(
        ActionResult<ContractQuery> result, string propertyName, TPropType? propertyValue = default)
        where TResultType : ObjectResult
    {
        var objResult = Assert.IsType<TResultType>(result.Result);
        Assert.NotNull(objResult.Value);

        var propValue = objResult.Value
            .GetType()
            .GetProperty(propertyName, typeof(TPropType))?
            .GetValue(objResult.Value);

        if (propertyValue is not null)
        {
            var typedPropValue = Assert.IsType<TPropType>(propValue);
            Assert.NotNull(typedPropValue);
            Assert.Equal(propertyValue, typedPropValue);
        }
        else
        {
            Assert.NotNull(propValue);
        }
    }

    #endregion
}