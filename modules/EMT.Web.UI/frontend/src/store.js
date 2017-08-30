import Reducers from './reducers/'
import { createStore, applyMiddleware } from 'redux'
import thunkMiddleware from 'redux-thunk'

import 'whatwg-fetch'

import { actionChartDataChange, actionCountersUpdate, actionOrgStructureChange, actionFetchUpdatesForCounters } from './actions/'
import { getRandomArbitrary, generateDummyData } from './helpers'

import moment from 'moment'

const defaultState = {
    counters: [],
    data: {},
    orgData: [],
    selected_counters: [],
    dateFrom: moment().add(-1, 'days'),
    dateTo: moment()
}

const store = createStore(
    Reducers,
    defaultState,
    applyMiddleware(thunkMiddleware)
)

// ===== Loading init state ============================
Promise.all([
    fetch(`${API_HOST}/api/counters`).then(d => d.json()),
    fetch(`${API_HOST}/api/orgstructure`).then(d => d.json())
]).then(resArr => {
    store.dispatch(actionCountersUpdate(resArr[0].filter(el => el.name)))
    store.dispatch(actionOrgStructureChange(resArr[1]))
})

// setInterval(() => {
//     const dataObj = {}
//     // store.getState().selected_counters.forEach(c => {
//     //     dataObj[c] = generateDummyData(500)
//     // })

//     store.dispatch(chartDataChange(dataObj, 11))
// }, 3000)

// const dataObj = {
//     11: generateDummyData(250)
// }
// store.dispatch(chartDataChange(dataObj, 11))

// console.log('generateDummyData: ', generateDummyData(10))

const reqInProgress = new Set()
$.connection.counterValueHub.client.newValues = (counters, after) => {
    console.log("[SignalR] counters:", counters, "after:", after);
    store.dispatch(actionFetchUpdatesForCounters(counters, after, reqInProgress))
}

$.connection.hub.url = `${HOST}/signalr`

$.connection.hub.start()
    .done(() => console.log("[SignalR] Connection: done"))
    .fail(() => console.log("[SignalR] Connection: fail"))

export default store
