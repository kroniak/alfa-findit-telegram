import {RESULTS_FETCH_FAILED, RESULTS_FETCH_STARTED, RESULTS_FETCH_SUCCESS} from "../actions/types";

const initialState = {
    data: [],
    error: null,
    isLoading: false
};

export default (state = initialState, {type, payload}) => {
    switch (type) {
        case RESULTS_FETCH_STARTED:
            return {
                ...state,
                isLoading: state.data.length === 0
            };

        case RESULTS_FETCH_FAILED:
            return {
                ...state,
                error: payload
            };

        case RESULTS_FETCH_SUCCESS:
            return {
                ...state,
                data: payload,
                error: null,
                isLoading: false
            };

        default:
            return state;
    }
};
