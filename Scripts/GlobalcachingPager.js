function setupGlobalcachingPage(CurrentPage, PageCount, TotalCount, pagerElementName, pageRequestFunction) {
    $(pagerElementName + 'Count').html(TotalCount);

    var maxPages = 5;
    //var pageRefLink = '#' + pagerElementName.substring(1);
    var pageRefLink = '#' + $(pagerElementName).closest("div").attr("id");

    // page block starting at a multiple of X pages
    var startPage;
    if (CurrentPage % maxPages == 0) {
        startPage = (Math.floor(CurrentPage / maxPages) * maxPages) - maxPages + 1;
    }
    else {
        startPage = (Math.floor(CurrentPage / maxPages) * maxPages) + 1;
    }
    if (CurrentPage > maxPages) {
        $(pagerElementName).append('<li><a href="' + pageRefLink + '" onclick="' + pageRequestFunction + '(1);">&lt;&lt;</a></li>');
        $(pagerElementName).append('<li><a href="' + pageRefLink + '" onclick="' + pageRequestFunction + '(' + (startPage - 1) + ');">&lt;</a></li>');
    }
    if (PageCount > 1) {
        for (var i = startPage; i < (startPage + maxPages) && i <= PageCount; i++) {
            if (i == CurrentPage) {
                $(pagerElementName).append('<li class="active"><a href="' + pageRefLink + '" onclick="' + pageRequestFunction + '(' + i + ');">' + i + '</a></li>');
            }
            else {
                $(pagerElementName).append('<li><a href="' + pageRefLink + '" onclick="' + pageRequestFunction + '(' + i + ');">' + i + '</a></li>');
            }
        }

        if ((startPage + maxPages) < PageCount) {
            $(pagerElementName).append('<li><a href="' + pageRefLink + '" onclick="' + pageRequestFunction + '(' + (startPage + maxPages) + ');">&gt;</a></li>');
            $(pagerElementName).append('<li><a href="' + pageRefLink + '" onclick="' + pageRequestFunction + '(' + (PageCount) + ');">&gt;&gt;</a></li>');
        }
    }
}

function initGlobalcachingPager(container, pagerElementName)
{
    $('#'+container).html('<table width="100%"><tr><td><ul class="pagination ' + pagerElementName + '"></ul></td><td align="right"><span class="label label-default ' + pagerElementName + 'Count">0</span></td></tr></table>');
}
