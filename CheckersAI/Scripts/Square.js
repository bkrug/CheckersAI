var Square = function (ele) {
    var self = this;
    self.$elt = $(ele);
    self.RegisterObject = function () {
        if (self.$elt.data('object') == null)
            self.$elt.data('object', self);
    };
    self.RegisterObject();

    var $board = self.$elt.closest('.js-board');
    var $dashboard = self.$elt.closest('.js-game-dashboard');

    self.Init = function () {
        self.$elt.mousedown(self.MouseDown);
        self.$elt.mouseup(self.MouseUp);
        self.$elt.mouseenter(self.HoverIn);
    };

    self.MouseDown = function (e) {
        if (Move.Started())
            CompleteMove();
        else
            BeginMove();
    };

    var BeginMove = function () {
        if (Move.HumanTeam == self.$elt.data("down")) {
            Move.StartRow = self.$elt.data('row');
            Move.StartColumn = self.$elt.data('column');
            Move.StopRow = self.$elt.data('row');
            Move.StopColumn = self.$elt.data('column');
            self.$elt.addClass('ui-high');
        }
    };

    var CompleteMove = function () {
        $.ajax({
            url: "/Board/MovePiece",
            data: {
                row: Move.StartRow,
                column: Move.StartColumn,
                move: Move.Stringify()
            },
            type: "POST",
            success: function (data) { $dashboard.data('object').DisplayNewMove(data, true); }
        });
        Move.Reset();
    };

    self.HoverIn = function (e) {
        if (!Move.Started())
            return;
        if (self.$elt.data('row') == Move.StopRow && self.$elt.data('column') == Move.StopColumn && Move.Steps.length > 0)
            return;
        var squares = GetSquares();
        var $adjacent = squares ? squares.adjacent : null;
        var $jump = squares ? squares.next : null;
        var moveAmount = squares ? squares.moveAmount : null;
        var direction = GetDirection();
        if (Move.Steps.length > 0 && !Move.Steps[0].Jump)
            CancelMove();
        else if (moveAmount == 1 && LegalSingle($adjacent, direction)) {
            $adjacent.addClass('ui-high');
            Move.Steps.push({ Direction: direction, Jump: false });
            Move.StopRow = $adjacent.data('row');
            Move.StopColumn = $adjacent.data('column');
        }
        else if ((moveAmount == 1 || moveAmount == 2) && LegalJump($adjacent, $jump, direction)) {
            $adjacent.addClass('ui-high');
            $jump.addClass('ui-high');
            Move.Steps.push({ Direction: direction, Jump: true });
            Move.StopRow = $jump.data('row');
            Move.StopColumn = $jump.data('column');
        }
        else {
            CancelMove();
        }
    };

    var GetDirection = function () {
        var moveDown = self.$elt.data('row') > Move.StopRow;
        var moveRight = self.$elt.data('column') > Move.StopColumn;
        if (moveDown)
            if (moveRight)
                return Direction.DOWN_RIGHT;
            else
                return Direction.DOWN_LEFT;
        else
            if (moveRight)
                return Direction.UP_RIGHT;
            else
                return Direction.UP_LEFT;
    };

    var GetSquares = function () {
        var vMove = self.$elt.data('row') - Move.StopRow;
        var hMove = self.$elt.data('column') - Move.StopColumn;
        if (Math.abs(vMove) != Math.abs(hMove))
            return null;
        var vDirection = vMove > 0 ? 1 : -1;
        var hDirection = hMove > 0 ? 1 : -1;
        var $adjacent = $board.find('.r' + (Move.StopRow + vDirection) + 'c' + (Move.StopColumn + hDirection));
        var $next = $board.find('.r' + (Move.StopRow + 2 * vDirection) + 'c' + (Move.StopColumn + 2 * hDirection));
        return { adjacent: $adjacent, next: $next, moveAmount: vMove };
    };

    var LegalSingle = function ($adjacent, direction) {
        if (Move.Steps.length > 0)
            return false;
        if (!$adjacent)
            return false;
        if ($adjacent.data('down') != null)
            return false;
        return IsDirectionLegal(direction);
    };

    var LegalJump = function ($adjacent, $jump, direction) {
        if (!$adjacent || !$jump)
            return false;
        if ($adjacent.data('down') == null || $adjacent.data('down') == Move.HumanTeam)
            return false;
        if ($jump.data('down') != null)
            return false;
        return IsDirectionLegal(direction);
    };

    var IsDirectionLegal = function (direction) {
        var $movingPiece = $board.find('.r' + Move.StartRow + 'c' + Move.StartColumn);
        if ($movingPiece.data('king') == false)
            if (Move.HumanTeam && direction != Direction.DOWN_LEFT && direction != Direction.DOWN_RIGHT)
                return false;
            else if (!Move.HumanTeam && direction != Direction.UP_LEFT && direction != Direction.UP_RIGHT)
                return false;
        return true;
    };

    var CancelMove = function () {
        Move.Reset();
        $board.find('.ui-high').removeClass('ui-high');
    }
};