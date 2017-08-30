import { connect } from 'react-redux'

import Header from './header.component'
import {
    actionDateFromChange,
    actionDateToChange,
    actionFetchCounterData
} from '../../actions/'

const mapStateToProps = state => {
    return {
        dateFrom: state.dateFrom,
        dateTo: state.dateTo
    }
}

const mapDispatchToProps = dispatch => {
    return {
        handleDateFromChange: date => {
            dispatch(actionDateFromChange(date))
            dispatch(actionFetchCounterData())
        },
        handleDateToChange: date => {
            dispatch(actionDateToChange(date))
            dispatch(actionFetchCounterData())
        }
    }
}

export default connect(
    mapStateToProps,
    mapDispatchToProps
)(Header)
