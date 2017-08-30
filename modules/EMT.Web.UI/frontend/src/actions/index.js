export const actionChartDataChange = (data, counter, rewriteData) => {
    return {
        type: 'CHART_UPDATE_DATA',
        data,
        counter,
        rewriteData
    }
}

export const actionCountersUpdate = data => {
    return {
        type: 'COUNTERS_UPDATE',
        data
    }
}

export const actionSelectCounter = data => {
    return {
        type: 'COUNTER_SELECT',
        data
    }
}

export const actionOrgStructureChange = data => {
    return {
        type: 'ORG_STRUCTURE_CHANGE',
        data
    }
}

export const actionFetchCounterData = counter => {
    return (dispatch, getState) => {
        const { selected_counters, data, dateFrom, dateTo } = getState(),
            selectedCounters = new Set(selected_counters),
            fetchDataForCounter = (counter, rewriteData) => {
                fetch(`${API_HOST}/api/counters/${counter}/values?timeFrom=${dateFrom.valueOf()}&timeTo=${dateTo.valueOf()}`)
                    .then(d => d.json())
                    .then(d => {
                        dispatch(actionChartDataChange(d, counter, rewriteData))
                    })
            }

        if (selectedCounters.has(counter)){
            // Get data for one selected counter
            fetchDataForCounter(counter)
        } else if (!counter) {
            // Get data for all counters
            selected_counters.forEach(counter => {
                const rewriteData = true

                fetchDataForCounter(counter, rewriteData)
            })
        }
    }
}

export const actionFetchUpdatesForCounters = (counters, after, reqInProgress = new Set()) => {
    return (dispatch, getState) => {
        const { selected_counters } = getState()

        selected_counters
            .filter(sc => counters.indexOf(sc) > -1)
            .forEach(counter => {
                if (!reqInProgress.has(counter)) {
                    reqInProgress.add(counter)
                    fetch(`${API_HOST}/api/counters/${counter}/values?after=${after}`)
                        .then(res => res.json())
                        .then(d => {
                            reqInProgress.delete(counter)
                            dispatch(actionChartDataChange(d, counter))
                        })
                }
            })
    }
}

export const actionDateFromChange = data => {
    return {
        type: 'CHANGE_DATE_FROM',
        data
    }
}

export const actionDateToChange = data => {
    return {
        type: 'CHANGE_DATE_TO',
        data
    }
}

