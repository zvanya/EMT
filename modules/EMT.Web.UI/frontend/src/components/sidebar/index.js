import sideBar from './sidebar.component.js'
import { connect } from 'react-redux'

import { actionSelectCounter, actionFetchCounterData } from '../../actions/'

const mapStateToProps = state => {
    return {
        counters: state.counters,
        orgData: state.orgData,
        selected_counters: state.selected_counters
    }
}

const mapDispatchToProps = dispatch => {
    return {
        onCounterClick: counter => {
            dispatch(actionSelectCounter(counter))
            dispatch(actionFetchCounterData(counter))
        }
    }
}

export default connect(
    mapStateToProps,
    mapDispatchToProps
)(sideBar)
