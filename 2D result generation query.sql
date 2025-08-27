DECLARE @DrawDate date = '2025-08-27 00:00:00.000'
DECLARE @DrawType nvarchar(MAX) = '2D 9PM Draw'
DECLARE @FirstNumber int = 7
DECLARE @SecondNumber int = 6

SELECT
	A.id,
	C.FirstName + ' ' + C.LastName as [Admin],
	A.Bettor,
	A.FirstDigit,
	A.SecondDigit,
	CASE WHEN @FirstNumber = @SecondNumber THEN 'POMPIANG' ELSE 'STRAIGHT' END as WinType,
	A.Amount,
	CASE WHEN @FirstNumber = @SecondNumber THEN B.WinPompi ELSE B.WinStraight END AS WinPrize,
	A.DrawDate,
	A.DrawType
FROM tb_TwoD A
LEFT JOIN ltb_twoDValidAmounts B ON B.Amount=A.Amount
LEFT JOIN Users C ON C.id=A.UserID
WHERE 
	DrawDate=@DrawDate AND
	DrawType=@DrawType AND
	FirstDigit = @FirstNumber
	AND
	SecondDigit = @SecondNumber
	AND
	A.Type='S'

UNION ALL

SELECT
	A.id,
	C.FirstName + ' ' + C.LastName as [Admin],
	A.Bettor,
	A.FirstDigit,
	A.SecondDigit,
	CASE WHEN @FirstNumber = @SecondNumber THEN 'POMPIANG' ELSE 'RAMBLE' END as WinType,
	A.Amount,
	CASE WHEN @FirstNumber = @SecondNumber THEN B.WinPompi ELSE B.WinRamble END AS WinPrize,
	A.DrawDate,
	A.DrawType
FROM tb_TwoD A
LEFT JOIN ltb_twoDValidAmounts B ON B.Amount=A.Amount
LEFT JOIN Users C ON C.id=A.UserID
WHERE 
	DrawDate=@DrawDate AND
	DrawType=@DrawType AND
	((FirstDigit = @FirstNumber AND SecondDigit = @SecondNumber)
	OR
	(SecondDigit = @FirstNumber AND FirstDigit = @SecondNumber))
	AND
	A.Type='R'

--EF QUERY
var drawDate = new DateTime(2025, 8, 27);
var drawType = "2D 9PM Draw";
var firstNumber = 7;
var secondNumber = 6;
bool isPompiang = firstNumber == secondNumber;

var straightQuery = from a in context.TwoDs
                    join b in context.TwoDValidAmounts
                        on a.Amount equals b.Amount into gj
                    from b in gj.DefaultIfEmpty()
                    join c in context.Users
                        on a.UserId equals c.Id into userJoin
                    from c in userJoin.DefaultIfEmpty()
                    where a.DrawDate == drawDate
                          && a.DrawType == drawType
                          && a.FirstDigit == firstNumber
                          && a.SecondDigit == secondNumber
                          && a.Type == "S"
                    select new
                    {
                        a.Id,
                        Admin = (c.FirstName + " " + c.LastName).Trim(),
                        a.Bettor,
                        a.FirstDigit,
                        a.SecondDigit,
                        WinType = isPompiang ? "POMPIANG" : "STRAIGHT",
                        a.Amount,
                        WinPrize = isPompiang ? b.WinPompi : b.WinStraight,
                        a.DrawDate,
                        a.DrawType
                    };

var rambleQuery = from a in context.TwoDs
                  join b in context.TwoDValidAmounts
                      on a.Amount equals b.Amount into gj
                  from b in gj.DefaultIfEmpty()
                  join c in context.Users
                      on a.UserId equals c.Id into userJoin
                  from c in userJoin.DefaultIfEmpty()
                  where a.DrawDate == drawDate
                        && a.DrawType == drawType
                        && (
                            (a.FirstDigit == firstNumber && a.SecondDigit == secondNumber) ||
                            (a.FirstDigit == secondNumber && a.SecondDigit == firstNumber)
                           )
                        && a.Type == "R"
                  select new
                  {
                      a.Id,
                      Admin = (c.FirstName + " " + c.LastName).Trim(),
                      a.Bettor,
                      a.FirstDigit,
                      a.SecondDigit,
                      WinType = isPompiang ? "POMPIANG" : "RAMBLE",
                      a.Amount,
                      WinPrize = isPompiang ? b.WinPompi : b.WinRamble,
                      a.DrawDate,
                      a.DrawType
                  };

var result = straightQuery.Concat(rambleQuery).ToList();
	