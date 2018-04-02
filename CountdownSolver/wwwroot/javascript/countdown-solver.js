document.addEventListener("DOMContentLoaded", function () {

    //Create new Vue instance.
    new Vue({
        el: '#app',
            data: {
                vueReactiveObject: countdownSolver.vueReactiveObject
            }
        })

    countdownSolver.eventFunctions.setupEvents();
});

var countdownSolver = function () {

    var lettersGameResultsRaw = [];
    var numbersGameResultsRaw = [];

    /**
     * Items in this object can be watched by the Vue instance for any changes.
     * Vue requires variables etc be encased in an object for reactivity (change detection) to work.
     */
    var vueReactiveObject = {
        lettersGameResults: [],
        totalWordsFound: lettersGameResultsRaw.length,
        numbersGameResults: [],
        totalCalculationsFound: numbersGameResultsRaw.length,
        noCalculationsFound: false,
        noWordsFound: false
    }

    var eventFunctions = function () {

        function setupEvents() {

            $("#lettersGameForm").on("submit", function () {
                let inputLetters = document.getElementById("letters").value;
                inputLetters = inputLetters.toLowerCase();

                //remove all invalid characters
                inputLetters = inputLetters.replace(/[^A-Za-z\s!?]/g, '');

                let url = `/countdownsolver/countdownletters?letters=${inputLetters}`;

                //clear any results from previous executions, this makes it obvious to the user new results are displayed
                resetLettersGame();

                commonFunctions.showLoader("lettersGameSubmitButton");

                ajaxFunctions.ajaxReuqest(url)
                    .then(function (returnedWordsArray) {
                        countdownSolver.vueReactiveObject.totalWordsFound = returnedWordsArray.length;

                        if (returnedWordsArray.length == 0) {
                            countdownSolver.vueReactiveObject.noWordsFound = true;
                        } else {
                            countdownSolver.vueReactiveObject.noWordsFound = false;
                        }

                        countdownSolver.lettersGameResultsRaw = jQuery.extend(true, [], returnedWordsArray);
                        let wordsWithLetterCount = commonFunctions.addLetterCounts(returnedWordsArray);

                        //sort by letterCount descending
                        wordsWithLetterCount.sort(function (a, b) {
                            return b.letterCount - a.letterCount
                        });

                        //if more than 1000 elements only show the first 1000
                        if (wordsWithLetterCount.length > 1000) {
                            wordsWithLetterCount = wordsWithLetterCount.slice(0, 1000);
                        }

                        commonFunctions.clearArray(countdownSolver.vueReactiveObject.lettersGameResults);
                        countdownSolver.commonFunctions.pushToArray(wordsWithLetterCount, countdownSolver.vueReactiveObject.lettersGameResults);

                        commonFunctions.removeLoader("lettersGameSubmitButton", "Get words");
                    });
                return false;
            });

            $(document).on('click', '#lettersGameClearButton', function () {
                resetLettersGame(true);
            });

            $("#numbersGameForm").on("submit", function () {
                let formData = $("#numbersGameForm").serialize();
                let url = `/countdownsolver/countdownnumbers?${formData}`;

                //clear any results from previous executions, this makes it obvious to the user new results are displayed
                resetNumbersGame();

                commonFunctions.showLoader("numbersGameSubmitButton");

                ajaxFunctions.ajaxReuqest(url)
                    .then(function (returnedCalculationsArray) {
                        countdownSolver.vueReactiveObject.totalCalculationsFound = returnedCalculationsArray.length;

                        if (returnedCalculationsArray.length == 0) {
                            countdownSolver.vueReactiveObject.noCalculationsFound = true;
                        } else {
                            countdownSolver.vueReactiveObject.noCalculationsFound = false;
                        }

                        countdownSolver.numbersGameResultRaw = jQuery.extend(true, [], returnedCalculationsArray);

                        //if more than 1000 elements only show the first 1000
                        if (returnedCalculationsArray.length > 1000) {
                            returnedCalculationsArray = returnedCalculationsArray.slice(0, 1000);
                        }

                        commonFunctions.clearArray(countdownSolver.vueReactiveObject.numbersGameResults);
                        countdownSolver.commonFunctions.pushToArray(returnedCalculationsArray, countdownSolver.vueReactiveObject.numbersGameResults);

                        commonFunctions.removeLoader("numbersGameSubmitButton", "Solve");
                    });
                return false;
            });

            $(document).on('click', '#numbersGameClearButton', function () {
                resetNumbersGame(true);
            });

            $(document).on('click', '#downloadLettersGameResults', function () {
                commonFunctions.downloadToCsv(countdownSolver.lettersGameResultsRaw, "wordsfound", "Words")
            });

            $(document).on('click', '#downloadnumbersGameResults', function () {
                commonFunctions.downloadToCsv(countdownSolver.numbersGameResultRaw, "calculationsfound", "Calculations")
            });
        }

        function resetLettersGame(clearForm = false) {
            if (clearForm) {
                document.getElementById("lettersGameForm").reset();
            }
            countdownSolver.vueReactiveObject.totalWordsFound = 0;
            countdownSolver.vueReactiveObject.noWordsFound = false;
            commonFunctions.clearArray(countdownSolver.vueReactiveObject.lettersGameResults);
        }

        function resetNumbersGame(clearForm) {
            if (clearForm) {
                document.getElementById("numbersGameForm").reset();
            }
            countdownSolver.vueReactiveObject.totalCalculationsFound = 0;
            countdownSolver.vueReactiveObject.noCalculationsFound = false;
            commonFunctions.clearArray(countdownSolver.vueReactiveObject.numbersGameResults);
        }

        return {
            setupEvents: setupEvents
        };
    }();

    var ajaxFunctions = function () {

        function ajaxReuqest(url, headers = null, requestObject = null) {

            if (headers != null) {
                headers = new Headers();
            }

            if (requestObject != null) {
                requestObject =
                    {
                        method: 'GET',
                        headers: headers,
                        mode: 'cors',
                        cache: 'default'
                    };
            }

            return fetch(url, requestObject)
                .then(response => response.json()) // parses response to JSON
        }

        return {
            ajaxReuqest: ajaxReuqest
        };

    }();

    var commonFunctions = function () {

        function pushToArray(inputArray, arrayToPushInto) {
            inputArray.forEach(function (element) {
                arrayToPushInto.push(element);
            });
        }

        function clearArray(arrayToClear) {
            while (arrayToClear.length) {
                arrayToClear.pop();
            }
        }

        function addLetterCounts(inputArray) {
            var outputArray = [];
            inputArray.forEach(function (element) {
                outputArray.push({ "word": element, "letterCount": element.length });
            });
            return outputArray;
        }

        function downloadToCsv(arrayToDownload, filename, columnHeading) {
            arrayToDownload.unshift(columnHeading);
            var csv = arrayToDownload.join(",\n");
            var csvElement = document.createElement('a');
            csvElement.href = 'data:attachment/csv,' + encodeURIComponent(csv);
            csvElement.target = '_blank';
            csvElement.download = `${filename}.csv`;
            document.body.appendChild(csvElement);
            csvElement.click();
        }

        function showLoader(elementId) {
            var element = document.getElementById(elementId);
            element.innerHTML = element.innerHTML + " <i class='fas fa-cog fa-spin'></i>";
            element.disabled = true;
        }

        function removeLoader(elementId, newInnerHtml) {
            var element = document.getElementById(elementId);
            element.innerHTML = newInnerHtml;
            element.disabled = false;
        }

        return {
            pushToArray: pushToArray,
            clearArray: clearArray,
            addLetterCounts: addLetterCounts,
            downloadToCsv: downloadToCsv,
            showLoader: showLoader,
            removeLoader: removeLoader
        };

    }();

    return {
        ajaxFunctions: ajaxFunctions,
        eventFunctions: eventFunctions,
        commonFunctions: commonFunctions,
        vueReactiveObject: vueReactiveObject
    };

}();