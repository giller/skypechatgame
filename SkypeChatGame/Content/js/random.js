(function ($) {
    $.fn.randomize = function (childElem) {
        return this.each(function () {
            var $this = $(this);
            var elems = $this.children(childElem);

            elems.sort(function () { return (Math.round(Math.random()) - 0.5); });

            $this.remove(childElem);

            for (var i = 0; i < elems.length; i++) {
                $this.append(elems[i]);
                $this.append(" ");
            }
        });
    }
})(jQuery);

$('div.authors').randomize('span');
