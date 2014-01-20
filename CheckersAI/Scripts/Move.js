var Move = function () {
    var self = this;

    Move.HumanTeam = true;
    Move.HumanTurn = true;
    Move.StartRow;
    Move.StartColumn;
    Move.StopRow;
    Move.StopColumn;
    Move.Steps;

    Move.Reset = function () {
        Move.StartRow = null;
        Move.StartColumn = null;
        Move.StopRow = null;
        Move.StopColumn = null;
        Move.Steps = [];
    };

    Move.Started = function () {
        return Move.StartRow != null && Move.StartColumn != null;
    };

    Move.IsValid = function () {
        return Move.Steps.length > 0;
    };

    Move.Stringify = function () {
        if (!Move.Started())
            return '';
        else
            return JSON.stringify({ Steps: Move.Steps });
    }

    Move.Reset();
};
Move();