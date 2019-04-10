import React, {Component} from 'react';
import Heading from 'arui-feather/heading';
import {connect} from "react-redux";
import {fetchResults} from "../actions/results";
import Plate from "arui-feather/plate";
import Paragraph from "arui-feather/paragraph";

const containerStyle = {
    "marginTop": "24px",
    "display": "grid"
};

const renderMedal = i => {
    if (i === 0) return <img src={"img/gold.svg"} alt="gold badge" className="badge"></img>
    else if (i === 1) return <img src={"img/silver.svg"} alt="silver badge" className="badge"></img>
    else if (i === 2) return <img src={"img/bronze.svg"} alt="bronze badge" className="badge"></img>
};

const declOfNum = (number, titles) => {
    const cases = [2, 0, 1, 1, 1, 2];
    return titles[(number % 100 > 4 && number % 100 < 20) ? 2 : cases[(number % 10 < 5) ? number % 10 : 5]];
};

const secToMinHour = sec => {
    const h = sec / 3600 ^ 0;
    const m = (sec - h * 3600) / 60 ^ 0;
    const s = sec - h * 3600 - m * 60;

    let str = "";
    if (h > 0) str = `${h} ч. `;
    else str = "";
    if (m > 0) str = str + `${m} мин. `;

    str = str + `${s} сек. `;
    return str;
};

const renderResult = ({name, points, seconds, phone}, i) =>
    (
        <div style={{"marginBottom": "10px"}} key={i}>
            <Plate isFlat={false} hasCloser={false}>
                {renderMedal(i)}
                <Heading size='m'>
                    {i + 1} место
                </Heading><Heading size='s'>
                {name} {phone}
            </Heading>
                <Paragraph>
                    Заработал {points} {declOfNum(points, ["балл", "балла", "баллов"])} за {secToMinHour(seconds)}
                </Paragraph>
            </Plate></div>
    );

class Results extends Component {
    constructor(props) {
        super(props);

        this.state = {
            timer: null
        }

        this.renderResults = this.renderResults.bind(this);
    }

    componentDidMount() {
        this.props.fetchResults();

        let timer = setInterval(this.props.fetchResults, 5000);

        this.setState({timer});
    }

    componentWillUnmount() {
        clearInterval(this.state.timer);
    }

    renderResults() {
        if (this.props.results) {
            return this.props.results.map((r, i) => renderResult(r, i));
        } else {
            return <div/>;
        }
    }

    render() {
        return (
            <div style={containerStyle}>
                {this.renderResults()}
            </div>
        );
    }
}

const mapStateToProps = state => ({
    results: state.results.data
});

const mapDispatchToProps = dispatch => ({
    fetchResults: () => dispatch(fetchResults())
});

export default connect(mapStateToProps, mapDispatchToProps)(Results);