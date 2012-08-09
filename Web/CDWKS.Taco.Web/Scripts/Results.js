$(document).ready(function () {
    $("#resultTable").tablesorter({
        // pass the headers argument and assing a object 
        headers: {
            0: {
                // disable it by setting the property sorter to false 
                sorter: false
            },

            1: {
                // disable it by setting the property sorter to false 
                sorter: false
            }
        }
    });
}
); 
   