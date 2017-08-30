import update from 'react/lib/update'

const charts = (state = {}, action) => {
    switch (action.type) {
        case 'CHART_UPDATE_DATA':
            let newCounterArr

            if (state.data[action.counter] && !action.rewriteData) {
                const dataLimit = 10000

                newCounterArr = state.data[action.counter].concat(action.data)

                if (newCounterArr.length > dataLimit) {
                    newCounterArr.splice(0, newCounterArr.length - dataLimit)
                }
            }

            return Object.assign({}, state, {
                'data': Object.assign({}, state.data, { [action.counter]: newCounterArr || action.data })
            })

        case 'CHANGE_DATE_FROM':
            return Object.assign({}, state, { 'dateFrom': action.data })

        case 'CHANGE_DATE_TO':
            return Object.assign({}, state, { 'dateTo': action.data })

        case 'COUNTERS_UPDATE':
            return Object.assign({}, state, { 'counters': action.data })

        case 'ORG_STRUCTURE_CHANGE':
            return Object.assign({}, state, { 'orgData': action.data })

        case 'COUNTER_SELECT':
            const counters = new Set(state.selected_counters)
            counters.has(action.data) ? counters.delete(action.data) : counters.add(action.data)

            return Object.assign({}, state, { 'selected_counters': [...counters] })
        default:
            return state
    }
}

export default charts
