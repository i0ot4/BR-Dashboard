/*
 * This combined file was created by the DataTables downloader builder:
 *   https://datatables.net/download
 *
 * To rebuild or modify this file with the latest versions of the included
 * software please visit:
 *   https://datatables.net/download/#bs5/r-2.5.0/sc-2.3.0
 *
 * Included libraries:
 *   Responsive 2.5.0, Scroller 2.3.0
 */

/*! Responsive 2.5.0
 * © SpryMedia Ltd - datatables.net/license
 */

(function( factory ){
	if ( typeof define === 'function' && define.amd ) {
		// AMD
		define( ['jquery', 'datatables.net'], function ( $ ) {
			return factory( $, window, document );
		} );
	}
	else if ( typeof exports === 'object' ) {
		// CommonJS
		var jq = require('jquery');
		var cjsRequires = function (root, $) {
			if ( ! $.fn.dataTable ) {
				require('datatables.net')(root, $);
			}
		};

		if (typeof window === 'undefined') {
			module.exports = function (root, $) {
				if ( ! root ) {
					// CommonJS environments without a window global must pass a
					// root. This will give an error otherwise
					root = window;
				}

				if ( ! $ ) {
					$ = jq( root );
				}

				cjsRequires( root, $ );
				return factory( $, root, root.document );
			};
		}
		else {
			cjsRequires( window, jq );
			module.exports = factory( jq, window, window.document );
		}
	}
	else {
		// Browser
		factory( jQuery, window, document );
	}
}(function( $, window, document, undefined ) {
'use strict';
var DataTable = $.fn.dataTable;



/**
 * @summary     Responsive
 * @description Responsive tables plug-in for DataTables
 * @version     2.5.0
 * @author      SpryMedia Ltd
 * @copyright   SpryMedia Ltd.
 *
 * This source file is free software, available under the following license:
 *   MIT license - http://datatables.net/license/mit
 *
 * This source file is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
 * or FITNESS FOR A PARTICULAR PURPOSE. See the license files for details.
 *
 * For details please refer to: http://www.datatables.net
 */

/**
 * Responsive is a plug-in for the DataTables library that makes use of
 * DataTables' ability to change the visibility of columns, changing the
 * visibility of columns so the displayed columns fit into the table container.
 * The end result is that complex tables will be dynamically adjusted to fit
 * into the viewport, be it on a desktop, tablet or mobile browser.
 *
 * Responsive for DataTables has two modes of operation, which can used
 * individually or combined:
 *
 * * Class name based control - columns assigned class names that match the
 *   breakpoint logic can be shown / hidden as required for each breakpoint.
 * * Automatic control - columns are automatically hidden when there is no
 *   room left to display them. Columns removed from the right.
 *
 * In additional to column visibility control, Responsive also has built into
 * options to use DataTables' child row display to show / hide the information
 * from the table that has been hidden. There are also two modes of operation
 * for this child row display:
 *
 * * Inline - when the control element that the user can use to show / hide
 *   child rows is displayed inside the first column of the table.
 * * Column - where a whole column is dedicated to be the show / hide control.
 *
 * Initialisation of Responsive is performed by:
 *
 * * Adding the class `responsive` or `dt-responsive` to the table. In this case
 *   Responsive will automatically be initialised with the default configuration
 *   options when the DataTable is created.
 * * Using the `responsive` option in the DataTables configuration options. This
 *   can also be used to specify the configuration options, or simply set to
 *   `true` to use the defaults.
 *
 *  @class
 *  @param {object} settings DataTables settings object for the host table
 *  @param {object} [opts] Configuration options
 *  @requires jQuery 1.7+
 *  @requires DataTables 1.10.3+
 *
 *  @example
 *      $('#example').DataTable( {
 *        responsive: true
 *      } );
 *    } );
 */
var Responsive = function (settings, opts) {
	// Sanity check that we are using DataTables 1.10 or newer
	if (!DataTable.versionCheck || !DataTable.versionCheck('1.10.10')) {
		throw 'DataTables Responsive requires DataTables 1.10.10 or newer';
	}

	this.s = {
		childNodeStore: {},
		columns: [],
		current: [],
		dt: new DataTable.Api(settings)
	};

	// Check if responsive has already been initialised on this table
	if (this.s.dt.settings()[0].responsive) {
		return;
	}

	// details is an object, but for simplicity the user can give it as a string
	// or a boolean
	if (opts && typeof opts.details === 'string') {
		opts.details = { type: opts.details };
	}
	else if (opts && opts.details === false) {
		opts.details = { type: false };
	}
	else if (opts && opts.details === true) {
		opts.details = { type: 'inline' };
	}

	this.c = $.extend(true, {}, Responsive.defaults, DataTable.defaults.responsive, opts);
	settings.responsive = this;
	this._constructor();
};

$.extend(Responsive.prototype, {
	/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
	 * Constructor
	 */

	/**
	 * Initialise the Responsive instance
	 *
	 * @private
	 */
	_constructor: function () {
		var that = this;
		var dt = this.s.dt;
		var dtPrivateSettings = dt.settings()[0];
		var oldWindowWidth = $(window).innerWidth();

		dt.settings()[0]._responsive = this;

		// Use DataTables' throttle function to avoid processor thrashing on
		// resize
		$(window).on(
			'resize.dtr orientationchange.dtr',
			DataTable.util.throttle(function () {
				// iOS has a bug whereby resize can fire when only scrolling
				// See: http://stackoverflow.com/questions/8898412
				var width = $(window).innerWidth();

				if (width !== oldWindowWidth) {
					that._resize();
					oldWindowWidth = width;
				}
			})
		);

		// DataTables doesn't currently trigger an event when a row is added, so
		// we need to hook into its private API to enforce the hidden rows when
		// new data is added
		dtPrivateSettings.oApi._fnCallbackReg(
			dtPrivateSettings,
			'aoRowCreatedCallback',
			function (tr, data, idx) {
				if ($.inArray(false, that.s.current) !== -1) {
					$('>td, >th', tr).each(function (i) {
						var idx = dt.column.index('toData', i);

						if (that.s.current[idx] === false) {
							$(this).css('display', 'none');
						}
					});
				}
			}
		);

		// Destroy event handler
		dt.on('destroy.dtr', function () {
			dt.off('.dtr');
			$(dt.table().body()).off('.dtr');
			$(window).off('resize.dtr orientationchange.dtr');
			dt.cells('.dtr-control').nodes().to$().removeClass('dtr-control');

			// Restore the columns that we've hidden
			$.each(that.s.current, function (i, val) {
				if (val === false) {
					that._setColumnVis(i, true);
				}
			});
		});

		// Reorder the breakpoints array here in case they have been added out
		// of order
		this.c.breakpoints.sort(function (a, b) {
			return a.width < b.width ? 1 : a.width > b.width ? -1 : 0;
		});

		this._classLogic();
		this._resizeAuto();

		// Details handler
		var details = this.c.details;

		if (details.type !== false) {
			that._detailsInit();

			// DataTables will trigger this event on every column it shows and
			// hides individually
			dt.on('column-visibility.dtr', function () {
				// Use a small debounce to allow multiple columns to be set together
				if (that._timer) {
					clearTimeout(that._timer);
				}

				that._timer = setTimeout(function () {
					that._timer = null;

					that._classLogic();
					that._resizeAuto();
					that._resize(true);

					that._redrawChildren();
				}, 100);
			});

			// Redraw the details box on each draw which will happen if the data
			// has changed. This is used until DataTables implements a native
			// `updated` event for rows
			dt.on('draw.dtr', function () {
				that._redrawChildren();
			});

			$(dt.table().node()).addClass('dtr-' + details.type);
		}

		dt.on('column-reorder.dtr', function (e, settings, details) {
			that._classLogic();
			that._resizeAuto();
			that._resize(true);
		});

		// Change in column sizes means we need to calc
		dt.on('column-sizing.dtr', function () {
			that._resizeAuto();
			that._resize();
		});

		// DT2 let's us tell it if we are hiding columns
		dt.on('column-calc.dt', function (e, d) {
			var curr = that.s.current;

			for (var i = 0; i < curr.length; i++) {
				var idx = d.visible.indexOf(i);

				if (curr[i] === false && idx >= 0) {
					d.visible.splice(idx, 1);
				}
			}
		});

		// On Ajax reload we want to reopen any child rows which are displayed
		// by responsive
		dt.on('preXhr.dtr', function () {
			var rowIds = [];
			dt.rows().every(function () {
				if (this.child.isShown()) {
					rowIds.push(this.id(true));
				}
			});

			dt.one('draw.dtr', function () {
				that._resizeAuto();
				that._resize();

				dt.rows(rowIds).every(function () {
					that._detailsDisplay(this, false);
				});
			});
		});

		dt.on('draw.dtr', function () {
			that._controlClass();
		}).on('init.dtr', function (e, settings, details) {
			if (e.namespace !== 'dt') {
				return;
			}

			that._resizeAuto();
			that._resize();

			// If columns were hidden, then DataTables needs to adjust the
			// column sizing
			if ($.inArray(false, that.s.current)) {
				dt.columns.adjust();
			}
		});

		// First pass - draw the table for the current viewport size
		this._resize();
	},

	/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
	 * Private methods
	 */

	/**
	 * Get and store nodes from a cell - use for node moving renderers
	 *
	 * @param {*} dt DT instance
	 * @param {*} row Row index
	 * @param {*} col Column index
	 */
	_childNodes: function (dt, row, col) {
		var name = row + '-' + col;

		if (this.s.childNodeStore[name]) {
			return this.s.childNodeStore[name];
		}

		// https://jsperf.com/childnodes-array-slice-vs-loop
		var nodes = [];
		var children = dt.cell(row, col).node().childNodes;
		for (var i = 0, ien = children.length; i < ien; i++) {
			nodes.push(children[i]);
		}

		this.s.childNodeStore[name] = nodes;

		return nodes;
	},

	/**
	 * Restore nodes from the cache to a table cell
	 *
	 * @param {*} dt DT instance
	 * @param {*} row Row index
	 * @param {*} col Column index
	 */
	_childNodesRestore: function (dt, row, col) {
		var name = row + '-' + col;

		if (!this.s.childNodeStore[name]) {
			return;
		}

		var node = dt.cell(row, col).node();
		var store = this.s.childNodeStore[name];
		var parent = store[0].parentNode;
		var parentChildren = parent.childNodes;
		var a = [];

		for (var i = 0, ien = parentChildren.length; i < ien; i++) {
			a.push(parentChildren[i]);
		}

		for (var j = 0, jen = a.length; j < jen; j++) {
			node.appendChild(a[j]);
		}

		this.s.childNodeStore[name] = undefined;
	},

	/**
	 * Calculate the visibility for the columns in a table for a given
	 * breakpoint. The result is pre-determined based on the class logic if
	 * class names are used to control all columns, but the width of the table
	 * is also used if there are columns which are to be automatically shown
	 * and hidden.
	 *
	 * @param  {string} breakpoint Breakpoint name to use for the calculation
	 * @return {array} Array of boolean values initiating the visibility of each
	 *   column.
	 *  @private
	 */
	_columnsVisiblity: function (breakpoint) {
		var dt = this.s.dt;
		var columns = this.s.columns;
		var i, ien;

		// Create an array that defines the column ordering based first on the
		// column's priority, and secondly the column index. This allows the
		// columns to be removed from the right if the priority matches
		var order = columns
			.map(function (col, idx) {
				return {
					columnIdx: idx,
					priority: col.priority
				};
			})
			.sort(function (a, b) {
				if (a.priority !== b.priority) {
					return a.priority - b.priority;
				}
				return a.columnIdx - b.columnIdx;
			});

		// Class logic - determine which columns are in this breakpoint based
		// on the classes. If no class control (i.e. `auto`) then `-` is used
		// to indicate this to the rest of the function
		var display = $.map(columns, function (col, i) {
			if (dt.column(i).visible() === false) {
				return 'not-visible';
			}
			return col.auto && col.minWidth === null
				? false
				: col.auto === true
				? '-'
				: $.inArray(breakpoint, col.includeIn) !== -1;
		});

		// Auto column control - first pass: how much width is taken by the
		// ones that must be included from the non-auto columns
		var requiredWidth = 0;
		for (i = 0, ien = display.length; i < ien; i++) {
			if (display[i] === true) {
				requiredWidth += columns[i].minWidth;
			}
		}

		// Second pass, use up any remaining width for other columns. For
		// scrolling tables we need to subtract the width of the scrollbar. It
		// may not be requires which makes this sub-optimal, but it would
		// require another full redraw to make complete use of those extra few
		// pixels
		var scrolling = dt.settings()[0].oScroll;
		var bar = scrolling.sY || scrolling.sX ? scrolling.iBarWidth : 0;
		var widthAvailable = dt.table().container().offsetWidth - bar;
		var usedWidth = widthAvailable - requiredWidth;

		// Control column needs to always be included. This makes it sub-
		// optimal in terms of using the available with, but to stop layout
		// thrashing or overflow. Also we need to account for the control column
		// width first so we know how much width is available for the other
		// columns, since the control column might not be the first one shown
		for (i = 0, ien = display.length; i < ien; i++) {
			if (columns[i].control) {
				usedWidth -= columns[i].minWidth;
			}
		}

		// Allow columns to be shown (counting by priority and then right to
		// left) until we run out of room
		var empty = false;
		for (i = 0, ien = order.length; i < ien; i++) {
			var colIdx = order[i].columnIdx;

			if (display[colIdx] === '-' && !columns[colIdx].control && columns[colIdx].minWidth) {
				// Once we've found a column that won't fit we don't let any
				// others display either, or columns might disappear in the
				// middle of the table
				if (empty || usedWidth - columns[colIdx].minWidth < 0) {
					empty = true;
					display[colIdx] = false;
				}
				else {
					display[colIdx] = true;
				}

				usedWidth -= columns[colIdx].minWidth;
			}
		}

		// Determine if the 'control' column should be shown (if there is one).
		// This is the case when there is a hidden column (that is not the
		// control column). The two loops look inefficient here, but they are
		// trivial and will fly through. We need to know the outcome from the
		// first , before the action in the second can be taken
		var showControl = false;

		for (i = 0, ien = columns.length; i < ien; i++) {
			if (!columns[i].control && !columns[i].never && display[i] === false) {
				showControl = true;
				break;
			}
		}

		for (i = 0, ien = columns.length; i < ien; i++) {
			if (columns[i].control) {
				display[i] = showControl;
			}

			// Replace not visible string with false from the control column detection above
			if (display[i] === 'not-visible') {
				display[i] = false;
			}
		}

		// Finally we need to make sure that there is at least one column that
		// is visible
		if ($.inArray(true, display) === -1) {
			display[0] = true;
		}

		return display;
	},

	/**
	 * Create the internal `columns` array with information about the columns
	 * for the table. This includes determining which breakpoints the column
	 * will appear in, based upon class names in the column, which makes up the
	 * vast majority of this method.
	 *
	 * @private
	 */
	_classLogic: function () {
		var that = this;
		var calc = {};
		var breakpoints = this.c.breakpoints;
		var dt = this.s.dt;
		var columns = dt
			.columns()
			.eq(0)
			.map(function (i) {
				var column = this.column(i);
				var className = column.header().className;
				var priority = dt.settings()[0].aoColumns[i].responsivePriority;
				var dataPriority = column.header().getAttribute('data-priority');

				if (priority === undefined) {
					priority =
						dataPriority === undefined || dataPriority === null
							? 10000
							: dataPriority * 1;
				}

				return {
					className: className,
					includeIn: [],
					auto: false,
					control: false,
					never: className.match(/\b(dtr\-)?never\b/) ? true : false,
					priority: priority
				};
			});

		// Simply add a breakpoint to `includeIn` array, ensuring that there are
		// no duplicates
		var add = function (colIdx, name) {
			var includeIn = columns[colIdx].includeIn;

			if ($.inArray(name, includeIn) === -1) {
				includeIn.push(name);
			}
		};

		var column = function (colIdx, name, operator, matched) {
			var size, i, ien;

			if (!operator) {
				columns[colIdx].includeIn.push(name);
			}
			else if (operator === 'max-') {
				// Add this breakpoint and all smaller
				size = that._find(name).width;

				for (i = 0, ien = breakpoints.length; i < ien; i++) {
					if (breakpoints[i].width <= size) {
						add(colIdx, breakpoints[i].name);
					}
				}
			}
			else if (operator === 'min-') {
				// Add this breakpoint and all larger
				size = that._find(name).width;

				for (i = 0, ien = breakpoints.length; i < ien; i++) {
					if (breakpoints[i].width >= size) {
						add(colIdx, breakpoints[i].name);
					}
				}
			}
			else if (operator === 'not-') {
				// Add all but this breakpoint
				for (i = 0, ien = breakpoints.length; i < ien; i++) {
					if (breakpoints[i].name.indexOf(matched) === -1) {
						add(colIdx, breakpoints[i].name);
					}
				}
			}
		};

		// Loop over each column and determine if it has a responsive control
		// class
		columns.each(function (col, i) {
			var classNames = col.className.split(' ');
			var hasClass = false;

			// Split the class name up so multiple rules can be applied if needed
			for (var k = 0, ken = classNames.length; k < ken; k++) {
				var className = classNames[k].trim();

				if (className === 'all' || className === 'dtr-all') {
					// Include in all
					hasClass = true;
					col.includeIn = $.map(breakpoints, function (a) {
						return a.name;
					});
					return;
				}
				else if (className === 'none' || className === 'dtr-none' || col.never) {
					// Include in none (default) and no auto
					hasClass = true;
					return;
				}
				else if (className === 'control' || className === 'dtr-control') {
					// Special column that is only visible, when one of the other
					// columns is hidden. This is used for the details control
					hasClass = true;
					col.control = true;
					return;
				}

				$.each(breakpoints, function (j, breakpoint) {
					// Does this column have a class that matches this breakpoint?
					var brokenPoint = breakpoint.name.split('-');
					var re = new RegExp(
						'(min\\-|max\\-|not\\-)?(' + brokenPoint[0] + ')(\\-[_a-zA-Z0-9])?'
					);
					var match = className.match(re);

					if (match) {
						hasClass = true;

						if (match[2] === brokenPoint[0] && match[3] === '-' + brokenPoint[1]) {
							// Class name matches breakpoint name fully
							column(i, breakpoint.name, match[1], match[2] + match[3]);
						}
						else if (match[2] === brokenPoint[0] && !match[3]) {
							// Class name matched primary breakpoint name with no qualifier
							column(i, breakpoint.name, match[1], match[2]);
						}
					}
				});
			}

			// If there was no control class, then automatic sizing is used
			if (!hasClass) {
				col.auto = true;
			}
		});

		this.s.columns = columns;
	},

	/**
	 * Update the cells to show the correct control class / button
	 * @private
	 */
	_controlClass: function () {
		if (this.c.details.type === 'inline') {
			var dt = this.s.dt;
			var columnsVis = this.s.current;
			var firstVisible = $.inArray(true, columnsVis);

			// Remove from any cells which shouldn't have it
			dt.cells(
				null,
				function (idx) {
					return idx !== firstVisible;
				},
				{ page: 'current' }
			)
				.nodes()
				.to$()
				.filter('.dtr-control')
				.removeClass('dtr-control');

			dt.cells(null, firstVisible, { page: 'current' }).nodes().to$().addClass('dtr-control');
		}
	},

	/**
	 * Show the details for the child row
	 *
	 * @param  {DataTables.Api} row    API instance for the row
	 * @param  {boolean}        update Update flag
	 * @private
	 */
	_detailsDisplay: function (row, update) {
		var that = this;
		var dt = this.s.dt;
		var details = this.c.details;
		var event = function (res) {
			$(row.node()).toggleClass('parent', res !== false);
			$(dt.table().node()).triggerHandler('responsive-display.dt', [dt, row, res, update]);
		};

		if (details && details.type !== false) {
			var renderer =
				typeof details.renderer === 'string'
					? Responsive.renderer[details.renderer]()
					: details.renderer;

			var res = details.display(
				row,
				update,
				function () {
					return renderer.call(that, dt, row[0], that._detailsObj(row[0]));
				},
				function () {
					event(false);
				}
			);

			if (typeof res === 'boolean') {
				event(res);
			}
		}
	},

	/**
	 * Initialisation for the details handler
	 *
	 * @private
	 */
	_detailsInit: function () {
		var that = this;
		var dt = this.s.dt;
		var details = this.c.details;

		// The inline type always uses the first child as the target
		if (details.type === 'inline') {
			details.target = 'td.dtr-control, th.dtr-control';
		}

		// Keyboard accessibility
		dt.on('draw.dtr', function () {
			that._tabIndexes();
		});
		that._tabIndexes(); // Initial draw has already happened

		$(dt.table().body()).on('keyup.dtr', 'td, th', function (e) {
			if (e.keyCode === 13 && $(this).data('dtr-keyboard')) {
				$(this).click();
			}
		});

		// type.target can be a string jQuery selector or a column index
		var target = details.target;
		var selector = typeof target === 'string' ? target : 'td, th';

		if (target !== undefined || target !== null) {
			// Click handler to show / hide the details rows when they are available
			$(dt.table().body()).on('click.dtr mousedown.dtr mouseup.dtr', selector, function (e) {
				// If the table is not collapsed (i.e. there is no hidden columns)
				// then take no action
				if (!$(dt.table().node()).hasClass('collapsed')) {
					return;
				}

				// Check that the row is actually a DataTable's controlled node
				if ($.inArray($(this).closest('tr').get(0), dt.rows().nodes().toArray()) === -1) {
					return;
				}

				// For column index, we determine if we should act or not in the
				// handler - otherwise it is already okay
				if (typeof target === 'number') {
					var targetIdx = target < 0 ? dt.columns().eq(0).length + target : target;

					if (dt.cell(this).index().column !== targetIdx) {
						return;
					}
				}

				// $().closest() includes itself in its check
				var row = dt.row($(this).closest('tr'));

				// Check event type to do an action
				if (e.type === 'click') {
					// The renderer is given as a function so the caller can execute it
					// only when they need (i.e. if hiding there is no point is running
					// the renderer)
					that._detailsDisplay(row, false);
				}
				else if (e.type === 'mousedown') {
					// For mouse users, prevent the focus ring from showing
					$(this).css('outline', 'none');
				}
				else if (e.type === 'mouseup') {
					// And then re-allow at the end of the click
					$(this).trigger('blur').css('outline', '');
				}
			});
		}
	},

	/**
	 * Get the details to pass to a renderer for a row
	 * @param  {int} rowIdx Row index
	 * @private
	 */
	_detailsObj: function (rowIdx) {
		var that = this;
		var dt = this.s.dt;

		return $.map(this.s.columns, function (col, i) {
			// Never and control columns should not be passed to the renderer
			if (col.never || col.control) {
				return;
			}

			var dtCol = dt.settings()[0].aoColumns[i];

			return {
				className: dtCol.sClass,
				columnIndex: i,
				data: dt.cell(rowIdx, i).render(that.c.orthogonal),
				hidden: dt.column(i).visible() && !that.s.current[i],
				rowIndex: rowIdx,
				title: dtCol.sTitle !== null ? dtCol.sTitle : $(dt.column(i).header()).text()
			};
		});
	},

	/**
	 * Find a breakpoint object from a name
	 *
	 * @param  {string} name Breakpoint name to find
	 * @return {object}      Breakpoint description object
	 * @private
	 */
	_find: function (name) {
		var breakpoints = this.c.breakpoints;

		for (var i = 0, ien = breakpoints.length; i < ien; i++) {
			if (breakpoints[i].name === name) {
				return breakpoints[i];
			}
		}
	},

	/**
	 * Re-create the contents of the child rows as the display has changed in
	 * some way.
	 *
	 * @private
	 */
	_redrawChildren: function () {
		var that = this;
		var dt = this.s.dt;

		dt.rows({ page: 'current' }).iterator('row', function (settings, idx) {
			that._detailsDisplay(dt.row(idx), true);
		});
	},

	/**
	 * Alter the table display for a resized viewport. This involves first
	 * determining what breakpoint the window currently is in, getting the
	 * column visibilities to apply and then setting them.
	 *
	 * @param  {boolean} forceRedraw Force a redraw
	 * @private
	 */
	_resize: function (forceRedraw) {
		var that = this;
		var dt = this.s.dt;
		var width = $(window).innerWidth();
		var breakpoints = this.c.breakpoints;
		var breakpoint = breakpoints[0].name;
		var columns = this.s.columns;
		var i, ien;
		var oldVis = this.s.current.slice();

		// Determine what breakpoint we are currently at
		for (i = breakpoints.length - 1; i >= 0; i--) {
			if (width <= breakpoints[i].width) {
				breakpoint = breakpoints[i].name;
				break;
			}
		}

		// Show the columns for that break point
		var columnsVis = this._columnsVisiblity(breakpoint);
		this.s.current = columnsVis;

		// Set the class before the column visibility is changed so event
		// listeners know what the state is. Need to determine if there are
		// any columns that are not visible but can be shown
		var collapsedClass = false;

		for (i = 0, ien = columns.length; i < ien; i++) {
			if (
				columnsVis[i] === false &&
				!columns[i].never &&
				!columns[i].control &&
				!dt.column(i).visible() === false
			) {
				collapsedClass = true;
				break;
			}
		}

		$(dt.table().node()).toggleClass('collapsed', collapsedClass);

		var changed = false;
		var visible = 0;

		dt.columns()
			.eq(0)
			.each(function (colIdx, i) {
				if (columnsVis[i] === true) {
					visible++;
				}

				if (forceRedraw || columnsVis[i] !== oldVis[i]) {
					changed = true;
					that._setColumnVis(colIdx, columnsVis[i]);
				}
			});

		// Always need to update the display, regardless of if it has changed or not, so nodes
		// can be re-inserted for listHiddenNodes
		this._redrawChildren();

		if (changed) {
			// Inform listeners of the change
			$(dt.table().node()).trigger('responsive-resize.dt', [dt, this.s.current]);

			// If no records, update the "No records" display element
			if (dt.page.info().recordsDisplay === 0) {
				$('td', dt.table().body()).eq(0).attr('colspan', visible);
			}
		}

		that._controlClass();
	},

	/**
	 * Determine the width of each column in the table so the auto column hiding
	 * has that information to work with. This method is never going to be 100%
	 * perfect since column widths can change slightly per page, but without
	 * seriously compromising performance this is quite effective.
	 *
	 * @private
	 */
	_resizeAuto: function () {
		var dt = this.s.dt;
		var columns = this.s.columns;
		var that = this;

		// Are we allowed to do auto sizing?
		if (!this.c.auto) {
			return;
		}

		// Are there any columns that actually need auto-sizing, or do they all
		// have classes defined
		if (
			$.inArray(
				true,
				$.map(columns, function (c) {
					return c.auto;
				})
			) === -1
		) {
			return;
		}

		// Need to restore all children. They will be reinstated by a re-render
		if (!$.isEmptyObject(this.s.childNodeStore)) {
			$.each(this.s.childNodeStore, function (key) {
				var idx = key.split('-');

				that._childNodesRestore(dt, idx[0] * 1, idx[1] * 1);
			});
		}

		// Clone the table with the current data in it
		var tableWidth = dt.table().node().offsetWidth;
		var columnWidths = dt.columns;
		var clonedTable = dt.table().node().cloneNode(false);
		var clonedHeader = $(dt.table().header().cloneNode(false)).appendTo(clonedTable);
		var clonedBody = $(dt.table().body()).clone(false, false).empty().appendTo(clonedTable); // use jQuery because of IE8

		clonedTable.style.width = 'auto';

		// Header
		var headerCells = dt
			.columns()
			.header()
			.filter(function (idx) {
				return dt.column(idx).visible();
			})
			.to$()
			.clone(false)
			.css('display', 'table-cell')
			.css('width', 'auto')
			.css('min-width', 0);

		// Body rows - we don't need to take account of DataTables' column
		// visibility since we implement our own here (hence the `display` set)
		$(clonedBody)
			.append($(dt.rows({ page: 'current' }).nodes()).clone(false))
			.find('th, td')
			.css('display', '');

		// Footer
		var footer = dt.table().footer();
		if (footer) {
			var clonedFooter = $(footer.cloneNode(false)).appendTo(clonedTable);
			var footerCells = dt
				.columns()
				.footer()
				.filter(function (idx) {
					return dt.column(idx).visible();
				})
				.to$()
				.clone(false)
				.css('display', 'table-cell');

			$('<tr/>').append(footerCells).appendTo(clonedFooter);
		}

		$('<tr/>').append(headerCells).appendTo(clonedHeader);

		// In the inline case extra padding is applied to the first column to
		// give space for the show / hide icon. We need to use this in the
		// calculation
		if (this.c.details.type === 'inline') {
			$(clonedTable).addClass('dtr-inline collapsed');
		}

		// It is unsafe to insert elements with the same name into the DOM
		// multiple times. For example, cloning and inserting a checked radio
		// clears the chcecked state of the original radio.
		$(clonedTable).find('[name]').removeAttr('name');

		// A position absolute table would take the table out of the flow of
		// our container element, bypassing the height and width (Scroller)
		$(clonedTable).css('position', 'relative');

		var inserted = $('<div/>')
			.css({
				width: 1,
				height: 1,
				overflow: 'hidden',
				clear: 'both'
			})
			.append(clonedTable);

		inserted.insertBefore(dt.table().node());

		// The cloned header now contains the smallest that each column can be
		headerCells.each(function (i) {
			var idx = dt.column.index('fromVisible', i);
			columns[idx].minWidth = this.offsetWidth || 0;
		});

		inserted.remove();
	},

	/**
	 * Get the state of the current hidden columns - controlled by Responsive only
	 */
	_responsiveOnlyHidden: function () {
		var dt = this.s.dt;

		return $.map(this.s.current, function (v, i) {
			// If the column is hidden by DataTables then it can't be hidden by
			// Responsive!
			if (dt.column(i).visible() === false) {
				return true;
			}
			return v;
		});
	},

	/**
	 * Set a column's visibility.
	 *
	 * We don't use DataTables' column visibility controls in order to ensure
	 * that column visibility can Responsive can no-exist. Since only IE8+ is
	 * supported (and all evergreen browsers of course) the control of the
	 * display attribute works well.
	 *
	 * @param {integer} col      Column index
	 * @param {boolean} showHide Show or hide (true or false)
	 * @private
	 */
	_setColumnVis: function (col, showHide) {
		var that = this;
		var dt = this.s.dt;
		var display = showHide ? '' : 'none'; // empty string will remove the attr

		$(dt.column(col).header()).css('display', display).toggleClass('dtr-hidden', !showHide);

		$(dt.column(col).footer()).css('display', display).toggleClass('dtr-hidden', !showHide);

		dt.column(col).nodes().to$().css('display', display).toggleClass('dtr-hidden', !showHide);

		// If the are child nodes stored, we might need to reinsert them
		if (!$.isEmptyObject(this.s.childNodeStore)) {
			dt.cells(null, col)
				.indexes()
				.each(function (idx) {
					that._childNodesRestore(dt, idx.row, idx.column);
				});
		}
	},

	/**
	 * Update the cell tab indexes for keyboard accessibility. This is called on
	 * every table draw - that is potentially inefficient, but also the least
	 * complex option given that column visibility can change on the fly. Its a
	 * shame user-focus was removed from CSS 3 UI, as it would have solved this
	 * issue with a single CSS statement.
	 *
	 * @private
	 */
	_tabIndexes: function () {
		var dt = this.s.dt;
		var cells = dt.cells({ page: 'current' }).nodes().to$();
		var ctx = dt.settings()[0];
		var target = this.c.details.target;

		cells.filter('[data-dtr-keyboard]').removeData('[data-dtr-keyboard]');

		if (typeof target === 'number') {
			dt.cells(null, target, { page: 'current' })
				.nodes()
				.to$()
				.attr('tabIndex', ctx.iTabIndex)
				.data('dtr-keyboard', 1);
		}
		else {
			// This is a bit of a hack - we need to limit the selected nodes to just
			// those of this table
			if (target === 'td:first-child, th:first-child') {
				target = '>td:first-child, >th:first-child';
			}

			$(target, dt.rows({ page: 'current' }).nodes())
				.attr('tabIndex', ctx.iTabIndex)
				.data('dtr-keyboard', 1);
		}
	}
});

/**
 * List of default breakpoints. Each item in the array is an object with two
 * properties:
 *
 * * `name` - the breakpoint name.
 * * `width` - the breakpoint width
 *
 * @name Responsive.breakpoints
 * @static
 */
Responsive.breakpoints = [
	{ name: 'desktop', width: Infinity },
	{ name: 'tablet-l', width: 1024 },
	{ name: 'tablet-p', width: 768 },
	{ name: 'mobile-l', width: 480 },
	{ name: 'mobile-p', width: 320 }
];

/**
 * Display methods - functions which define how the hidden data should be shown
 * in the table.
 *
 * @namespace
 * @name Responsive.defaults
 * @static
 */
Responsive.display = {
	childRow: function (row, update, render) {
		if (update) {
			if ($(row.node()).hasClass('parent')) {
				row.child(render(), 'child').show();

				return true;
			}
		}
		else {
			if (!row.child.isShown()) {
				row.child(render(), 'child').show();

				return true;
			}
			else {
				row.child(false);

				return false;
			}
		}
	},

	childRowImmediate: function (row, update, render) {
		if ((!update && row.child.isShown()) || !row.responsive.hasHidden()) {
			// User interaction and the row is show, or nothing to show
			row.child(false);

			return false;
		}
		else {
			// Display
			row.child(render(), 'child').show();

			return true;
		}
	},

	// This is a wrapper so the modal options for Bootstrap and jQuery UI can
	// have options passed into them. This specific one doesn't need to be a
	// function but it is for consistency in the `modal` name
	modal: function (options) {
		return function (row, update, render, closeCallback) {
			if (!update) {
				// Show a modal
				var close = function () {
					modal.remove(); // will tidy events for us
					$(document).off('keypress.dtr');
					$(row.node()).removeClass('parent');

					closeCallback();
				};

				var modal = $('<div class="dtr-modal"/>')
					.append(
						$('<div class="dtr-modal-display"/>')
							.append(
								$('<div class="dtr-modal-content"/>')
									.data('dtr-row-idx', row.index())
									.append(render())
							)
							.append(
								$('<div class="dtr-modal-close">&times;</div>').click(function () {
									close();
								})
							)
					)
					.append(
						$('<div class="dtr-modal-background"/>').click(function () {
							close();
						})
					)
					.appendTo('body');

				$(row.node()).addClass('parent');

				$(document).on('keyup.dtr', function (e) {
					if (e.keyCode === 27) {
						e.stopPropagation();

						close();
					}
				});
			}
			else {
				var modal = $('div.dtr-modal-content');

				if (modal.length && row.index() === modal.data('dtr-row-idx')) {
					modal.empty().append(render());
				}
				else {
					// Modal not shown, nothing to update
					return null;
				}
			}

			if (options && options.header) {
				$('div.dtr-modal-content').prepend('<h2>' + options.header(row) + '</h2>');
			}

			return true;
		};
	}
};

/**
 * Display methods - functions which define how the hidden data should be shown
 * in the table.
 *
 * @namespace
 * @name Responsive.defaults
 * @static
 */
Responsive.renderer = {
	listHiddenNodes: function () {
		return function (api, rowIdx, columns) {
			var that = this;
			var ul = $('<ul data-dtr-index="' + rowIdx + '" class="dtr-details"/>');
			var found = false;

			var data = $.each(columns, function (i, col) {
				if (col.hidden) {
					var klass = col.className ? 'class="' + col.className + '"' : '';

					$(
						'<li ' +
							klass +
							' data-dtr-index="' +
							col.columnIndex +
							'" data-dt-row="' +
							col.rowIndex +
							'" data-dt-column="' +
							col.columnIndex +
							'">' +
							'<span class="dtr-title">' +
							col.title +
							'</span> ' +
							'</li>'
					)
						.append(
							$('<span class="dtr-data"/>').append(
								that._childNodes(api, col.rowIndex, col.columnIndex)
							)
						) // api.cell( col.rowIndex, col.columnIndex ).node().childNodes ) )
						.appendTo(ul);

					found = true;
				}
			});

			return found ? ul : false;
		};
	},

	listHidden: function () {
		return function (api, rowIdx, columns) {
			var data = $.map(columns, function (col) {
				var klass = col.className ? 'class="' + col.className + '"' : '';

				return col.hidden
					? '<li ' +
							klass +
							' data-dtr-index="' +
							col.columnIndex +
							'" data-dt-row="' +
							col.rowIndex +
							'" data-dt-column="' +
							col.columnIndex +
							'">' +
							'<span class="dtr-title">' +
							col.title +
							'</span> ' +
							'<span class="dtr-data">' +
							col.data +
							'</span>' +
							'</li>'
					: '';
			}).join('');

			return data
				? $('<ul data-dtr-index="' + rowIdx + '" class="dtr-details"/>').append(data)
				: false;
		};
	},

	tableAll: function (options) {
		options = $.extend(
			{
				tableClass: ''
			},
			options
		);

		return function (api, rowIdx, columns) {
			var data = $.map(columns, function (col) {
				var klass = col.className ? 'class="' + col.className + '"' : '';

				return (
					'<tr ' +
					klass +
					' data-dt-row="' +
					col.rowIndex +
					'" data-dt-column="' +
					col.columnIndex +
					'">' +
					'<td>' +
					col.title +
					':' +
					'</td> ' +
					'<td>' +
					col.data +
					'</td>' +
					'</tr>'
				);
			}).join('');

			return $('<table class="' + options.tableClass + ' dtr-details" width="100%"/>').append(
				data
			);
		};
	}
};

/**
 * Responsive default settings for initialisation
 *
 * @namespace
 * @name Responsive.defaults
 * @static
 */
Responsive.defaults = {
	/**
	 * List of breakpoints for the instance. Note that this means that each
	 * instance can have its own breakpoints. Additionally, the breakpoints
	 * cannot be changed once an instance has been creased.
	 *
	 * @type {Array}
	 * @default Takes the value of `Responsive.breakpoints`
	 */
	breakpoints: Responsive.breakpoints,

	/**
	 * Enable / disable auto hiding calculations. It can help to increase
	 * performance slightly if you disable this option, but all columns would
	 * need to have breakpoint classes assigned to them
	 *
	 * @type {Boolean}
	 * @default  `true`
	 */
	auto: true,

	/**
	 * Details control. If given as a string value, the `type` property of the
	 * default object is set to that value, and the defaults used for the rest
	 * of the object - this is for ease of implementation.
	 *
	 * The object consists of the following properties:
	 *
	 * * `display` - A function that is used to show and hide the hidden details
	 * * `renderer` - function that is called for display of the child row data.
	 *   The default function will show the data from the hidden columns
	 * * `target` - Used as the selector for what objects to attach the child
	 *   open / close to
	 * * `type` - `false` to disable the details display, `inline` or `column`
	 *   for the two control types
	 *
	 * @type {Object|string}
	 */
	details: {
		display: Responsive.display.childRow,

		renderer: Responsive.renderer.listHidden(),

		target: 0,

		type: 'inline'
	},

	/**
	 * Orthogonal data request option. This is used to define the data type
	 * requested when Responsive gets the data to show in the child row.
	 *
	 * @type {String}
	 */
	orthogonal: 'display'
};

/*
 * API
 */
var Api = $.fn.dataTable.Api;

// Doesn't do anything - work around for a bug in DT... Not documented
Api.register('responsive()', function () {
	return this;
});

Api.register('responsive.index()', function (li) {
	li = $(li);

	return {
		column: li.data('dtr-index'),
		row: li.parent().data('dtr-index')
	};
});

Api.register('responsive.rebuild()', function () {
	return this.iterator('table', function (ctx) {
		if (ctx._responsive) {
			ctx._responsive._classLogic();
		}
	});
});

Api.register('responsive.recalc()', function () {
	return this.iterator('table', function (ctx) {
		if (ctx._responsive) {
			ctx._responsive._resizeAuto();
			ctx._responsive._resize();
		}
	});
});

Api.register('responsive.hasHidden()', function () {
	var ctx = this.context[0];

	return ctx._responsive
		? $.inArray(false, ctx._responsive._responsiveOnlyHidden()) !== -1
		: false;
});

Api.registerPlural('columns().responsiveHidden()', 'column().responsiveHidden()', function () {
	return this.iterator(
		'column',
		function (settings, column) {
			return settings._responsive
				? settings._responsive._responsiveOnlyHidden()[column]
				: false;
		},
		1
	);
});

/**
 * Version information
 *
 * @name Responsive.version
 * @static
 */
Responsive.version = '2.5.0';

$.fn.dataTable.Responsive = Responsive;
$.fn.DataTable.Responsive = Responsive;

// Attach a listener to the document which listens for DataTables initialisation
// events so we can automatically initialise
$(document).on('preInit.dt.dtr', function (e, settings, json) {
	if (e.namespace !== 'dt') {
		return;
	}

	if (
		$(settings.nTable).hasClass('responsive') ||
		$(settings.nTable).hasClass('dt-responsive') ||
		settings.oInit.responsive ||
		DataTable.defaults.responsive
	) {
		var init = settings.oInit.responsive;

		if (init !== false) {
			new Responsive(settings, $.isPlainObject(init) ? init : {});
		}
	}
});


return DataTable;
}));


/*! Bootstrap 5 integration for DataTables' Responsive
 * © SpryMedia Ltd - datatables.net/license
 */

(function( factory ){
	if ( typeof define === 'function' && define.amd ) {
		// AMD
		define( ['jquery', 'datatables.net-bs5', 'datatables.net-responsive'], function ( $ ) {
			return factory( $, window, document );
		} );
	}
	else if ( typeof exports === 'object' ) {
		// CommonJS
		var jq = require('jquery');
		var cjsRequires = function (root, $) {
			if ( ! $.fn.dataTable ) {
				require('datatables.net-bs5')(root, $);
			}

			if ( ! $.fn.dataTable.Responsive ) {
				require('datatables.net-responsive')(root, $);
			}
		};

		if (typeof window === 'undefined') {
			module.exports = function (root, $) {
				if ( ! root ) {
					// CommonJS environments without a window global must pass a
					// root. This will give an error otherwise
					root = window;
				}

				if ( ! $ ) {
					$ = jq( root );
				}

				cjsRequires( root, $ );
				return factory( $, root, root.document );
			};
		}
		else {
			cjsRequires( window, jq );
			module.exports = factory( jq, window, window.document );
		}
	}
	else {
		// Browser
		factory( jQuery, window, document );
	}
}(function( $, window, document, undefined ) {
'use strict';
var DataTable = $.fn.dataTable;



var _display = DataTable.Responsive.display;
var _original = _display.modal;
var _modal = $(
	'<div class="modal fade dtr-bs-modal" role="dialog">' +
		'<div class="modal-dialog" role="document">' +
		'<div class="modal-content">' +
		'<div class="modal-header">' +
		'<button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>' +
		'</div>' +
		'<div class="modal-body"/>' +
		'</div>' +
		'</div>' +
		'</div>'
);
var modal;

// Note this could be undefined at the time of initialisation - the
// DataTable.Responsive.bootstrap function can be used to set a different
// bootstrap object
var _bs = window.bootstrap;

DataTable.Responsive.bootstrap = function (bs) {
	_bs = bs;
};

_display.modal = function (options) {
	if (!modal) {
		modal = new _bs.Modal(_modal[0]);
	}

	return function (row, update, render, closeCallback) {
		if (!$.fn.modal) {
			return _original(row, update, render, closeCallback);
		}
		else {
			if (!update) {
				if (options && options.header) {
					var header = _modal.find('div.modal-header');
					var button = header.find('button').detach();

					header
						.empty()
						.append('<h4 class="modal-title">' + options.header(row) + '</h4>')
						.append(button);
				}

				_modal.find('div.modal-body').empty().append(render());

				_modal
					.data('dtr-row-idx', row.index())
					.one('hidden.bs.modal', closeCallback)
					.appendTo('body')
					.modal();

				modal.show();
			}
			else {
				if ($.contains(document, _modal[0]) && row.index() === _modal.data('dtr-row-idx')) {
					_modal.find('div.modal-body').empty().append(render());
				}
				else {
					// Modal not shown for this row - do nothing
					return null;
				}
			}

			return true;
		}
	};
};


return DataTable;
}));


/*! Scroller 2.3.0
 * © SpryMedia Ltd - datatables.net/license
 */

(function( factory ){
	if ( typeof define === 'function' && define.amd ) {
		// AMD
		define( ['jquery', 'datatables.net'], function ( $ ) {
			return factory( $, window, document );
		} );
	}
	else if ( typeof exports === 'object' ) {
		// CommonJS
		var jq = require('jquery');
		var cjsRequires = function (root, $) {
			if ( ! $.fn.dataTable ) {
				require('datatables.net')(root, $);
			}
		};

		if (typeof window === 'undefined') {
			module.exports = function (root, $) {
				if ( ! root ) {
					// CommonJS environments without a window global must pass a
					// root. This will give an error otherwise
					root = window;
				}

				if ( ! $ ) {
					$ = jq( root );
				}

				cjsRequires( root, $ );
				return factory( $, root, root.document );
			};
		}
		else {
			cjsRequires( window, jq );
			module.exports = factory( jq, window, window.document );
		}
	}
	else {
		// Browser
		factory( jQuery, window, document );
	}
}(function( $, window, document, undefined ) {
'use strict';
var DataTable = $.fn.dataTable;



/**
 * @summary     Scroller
 * @description Virtual rendering for DataTables
 * @version     2.3.0
 * @copyright   SpryMedia Ltd.
 *
 * This source file is free software, available under the following license:
 *   MIT license - http://datatables.net/license/mit
 *
 * This source file is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
 * or FITNESS FOR A PARTICULAR PURPOSE. See the license files for details.
 *
 * For details please refer to: http://www.datatables.net
 */

/**
 * Scroller is a virtual rendering plug-in for DataTables which allows large
 * datasets to be drawn on screen every quickly. What the virtual rendering means
 * is that only the visible portion of the table (and a bit to either side to make
 * the scrolling smooth) is drawn, while the scrolling container gives the
 * visual impression that the whole table is visible. This is done by making use
 * of the pagination abilities of DataTables and moving the table around in the
 * scrolling container DataTables adds to the page. The scrolling container is
 * forced to the height it would be for the full table display using an extra
 * element.
 *
 * Note that rows in the table MUST all be the same height. Information in a cell
 * which expands on to multiple lines will cause some odd behaviour in the scrolling.
 *
 * Scroller is initialised by simply including the letter 'S' in the sDom for the
 * table you want to have this feature enabled on. Note that the 'S' must come
 * AFTER the 't' parameter in `dom`.
 *
 * Key features include:
 *   <ul class="limit_length">
 *     <li>Speed! The aim of Scroller for DataTables is to make rendering large data sets fast</li>
 *     <li>Full compatibility with deferred rendering in DataTables for maximum speed</li>
 *     <li>Display millions of rows</li>
 *     <li>Integration with state saving in DataTables (scrolling position is saved)</li>
 *     <li>Easy to use</li>
 *   </ul>
 *
 *  @class
 *  @constructor
 *  @global
 *  @param {object} dt DataTables settings object or API instance
 *  @param {object} [opts={}] Configuration object for Scroller. Options
 *    are defined by {@link Scroller.defaults}
 *
 *  @requires jQuery 1.7+
 *  @requires DataTables 1.11.0+
 */
var Scroller = function (dt, opts) {
	/* Sanity check - you just know it will happen */
	if (!(this instanceof Scroller)) {
		alert(
			"Scroller warning: Scroller must be initialised with the 'new' keyword."
		);
		return;
	}

	if (opts === undefined) {
		opts = {};
	}

	var dtApi = $.fn.dataTable.Api(dt);

	/**
	 * Settings object which contains customisable information for the Scroller instance
	 * @namespace
	 * @private
	 * @extends Scroller.defaults
	 */
	this.s = {
		/**
		 * DataTables settings object
		 *  @type     object
		 *  @default  Passed in as first parameter to constructor
		 */
		dt: dtApi.settings()[0],

		/**
		 * DataTables API instance
		 *  @type     DataTable.Api
		 */
		dtApi: dtApi,

		/**
		 * Pixel location of the top of the drawn table in the viewport
		 *  @type     int
		 *  @default  0
		 */
		tableTop: 0,

		/**
		 * Pixel location of the bottom of the drawn table in the viewport
		 *  @type     int
		 *  @default  0
		 */
		tableBottom: 0,

		/**
		 * Pixel location of the boundary for when the next data set should be loaded and drawn
		 * when scrolling up the way.
		 *  @type     int
		 *  @default  0
		 *  @private
		 */
		redrawTop: 0,

		/**
		 * Pixel location of the boundary for when the next data set should be loaded and drawn
		 * when scrolling down the way. Note that this is actually calculated as the offset from
		 * the top.
		 *  @type     int
		 *  @default  0
		 *  @private
		 */
		redrawBottom: 0,

		/**
		 * Auto row height or not indicator
		 *  @type     bool
		 *  @default  0
		 */
		autoHeight: true,

		/**
		 * Number of rows calculated as visible in the visible viewport
		 *  @type     int
		 *  @default  0
		 */
		viewportRows: 0,

		/**
		 * setTimeout reference for state saving, used when state saving is enabled in the DataTable
		 * and when the user scrolls the viewport in order to stop the cookie set taking too much
		 * CPU!
		 *  @type     int
		 *  @default  0
		 */
		stateTO: null,

		stateSaveThrottle: function () {},

		/**
		 * setTimeout reference for the redraw, used when server-side processing is enabled in the
		 * DataTables in order to prevent DoSing the server
		 *  @type     int
		 *  @default  null
		 */
		drawTO: null,

		heights: {
			jump: null,
			page: null,
			virtual: null,
			scroll: null,

			/**
			 * Height of rows in the table
			 *  @type     int
			 *  @default  0
			 */
			row: null,

			/**
			 * Pixel height of the viewport
			 *  @type     int
			 *  @default  0
			 */
			viewport: null,
			labelHeight: 0,
			xbar: 0
		},

		topRowFloat: 0,
		scrollDrawDiff: null,
		loaderVisible: false,
		forceReposition: false,
		baseRowTop: 0,
		baseScrollTop: 0,
		mousedown: false,
		lastScrollTop: 0
	};

	// @todo The defaults should extend a `c` property and the internal settings
	// only held in the `s` property. At the moment they are mixed
	this.s = $.extend(this.s, Scroller.oDefaults, opts);

	// Workaround for row height being read from height object (see above comment)
	this.s.heights.row = this.s.rowHeight;

	/**
	 * DOM elements used by the class instance
	 * @private
	 * @namespace
	 *
	 */
	this.dom = {
		force: document.createElement('div'),
		label: $('<div class="dts_label">0</div>'),
		scroller: null,
		table: null,
		loader: null
	};

	// Attach the instance to the DataTables instance so it can be accessed in
	// future. Don't initialise Scroller twice on the same table
	if (this.s.dt.oScroller) {
		return;
	}

	this.s.dt.oScroller = this;

	/* Let's do it */
	this.construct();
};

$.extend(Scroller.prototype, {
	/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
	 * Public methods - to be exposed via the DataTables API
	 */

	/**
	 * Calculate and store information about how many rows are to be displayed
	 * in the scrolling viewport, based on current dimensions in the browser's
	 * rendering. This can be particularly useful if the table is initially
	 * drawn in a hidden element - for example in a tab.
	 *  @param {bool} [redraw=true] Redraw the table automatically after the recalculation, with
	 *    the new dimensions forming the basis for the draw.
	 *  @returns {void}
	 */
	measure: function (redraw) {
		if (this.s.autoHeight) {
			this._calcRowHeight();
		}

		var heights = this.s.heights;

		if (heights.row) {
			heights.viewport = this._parseHeight(
				$(this.dom.scroller).css('max-height')
			);

			this.s.viewportRows =
				parseInt(heights.viewport / heights.row, 10) + 1;
			this.s.dt._iDisplayLength =
				this.s.viewportRows * this.s.displayBuffer;
		}

		var label = this.dom.label.outerHeight();

		heights.xbar =
			this.dom.scroller.offsetHeight - this.dom.scroller.clientHeight;
		heights.labelHeight = label;

		if (redraw === undefined || redraw) {
			this.s.dt.oInstance.fnDraw(false);
		}
	},

	/**
	 * Get information about current displayed record range. This corresponds to
	 * the information usually displayed in the "Info" block of the table.
	 *
	 * @returns {object} info as an object:
	 *  {
	 *      start: {int}, // the 0-indexed record at the top of the viewport
	 *      end:   {int}, // the 0-indexed record at the bottom of the viewport
	 *  }
	 */
	pageInfo: function () {
		var dt = this.s.dt,
			iScrollTop = this.dom.scroller.scrollTop,
			iTotal = dt.fnRecordsDisplay(),
			iPossibleEnd = Math.ceil(
				this.pixelsToRow(
					iScrollTop + this.s.heights.viewport,
					false,
					this.s.ani
				)
			);

		return {
			start: Math.floor(this.pixelsToRow(iScrollTop, false, this.s.ani)),
			end: iTotal < iPossibleEnd ? iTotal - 1 : iPossibleEnd - 1
		};
	},

	/**
	 * Calculate the row number that will be found at the given pixel position
	 * (y-scroll).
	 *
	 * Please note that when the height of the full table exceeds 1 million
	 * pixels, Scroller switches into a non-linear mode for the scrollbar to fit
	 * all of the records into a finite area, but this function returns a linear
	 * value (relative to the last non-linear positioning).
	 *  @param {int} pixels Offset from top to calculate the row number of
	 *  @param {int} [intParse=true] If an integer value should be returned
	 *  @param {int} [virtual=false] Perform the calculations in the virtual domain
	 *  @returns {int} Row index
	 */
	pixelsToRow: function (pixels, intParse, virtual) {
		var diff = pixels - this.s.baseScrollTop;
		var row = virtual
			? (this._domain('physicalToVirtual', this.s.baseScrollTop) + diff) /
				this.s.heights.row
			: diff / this.s.heights.row + this.s.baseRowTop;

		return intParse || intParse === undefined ? parseInt(row, 10) : row;
	},

	/**
	 * Calculate the pixel position from the top of the scrolling container for
	 * a given row
	 *  @param {int} iRow Row number to calculate the position of
	 *  @returns {int} Pixels
	 */
	rowToPixels: function (rowIdx, intParse, virtual) {
		var pixels;
		var diff = rowIdx - this.s.baseRowTop;

		if (virtual) {
			pixels = this._domain('virtualToPhysical', this.s.baseScrollTop);
			pixels += diff * this.s.heights.row;
		}
		else {
			pixels = this.s.baseScrollTop;
			pixels += diff * this.s.heights.row;
		}

		return intParse || intParse === undefined
			? parseInt(pixels, 10)
			: pixels;
	},

	/**
	 * Calculate the row number that will be found at the given pixel position (y-scroll)
	 *  @param {int} row Row index to scroll to
	 *  @param {bool} [animate=true] Animate the transition or not
	 *  @returns {void}
	 */
	scrollToRow: function (row, animate) {
		var that = this;
		var ani = false;
		var px = this.rowToPixels(row);

		// We need to know if the table will redraw or not before doing the
		// scroll. If it will not redraw, then we need to use the currently
		// displayed table, and scroll with the physical pixels. Otherwise, we
		// need to calculate the table's new position from the virtual
		// transform.
		var preRows = ((this.s.displayBuffer - 1) / 2) * this.s.viewportRows;
		var drawRow = row - preRows;
		if (drawRow < 0) {
			drawRow = 0;
		}

		if (
			(px > this.s.redrawBottom || px < this.s.redrawTop) &&
			this.s.dt._iDisplayStart !== drawRow
		) {
			ani = true;
			px = this._domain('virtualToPhysical', row * this.s.heights.row);

			// If we need records outside the current draw region, but the new
			// scrolling position is inside that (due to the non-linear nature
			// for larger numbers of records), we need to force position update.
			if (this.s.redrawTop < px && px < this.s.redrawBottom) {
				this.s.forceReposition = true;
				animate = false;
			}
		}

		if (animate === undefined || animate) {
			this.s.ani = ani;
			$(this.dom.scroller).animate(
				{
					scrollTop: px
				},
				function () {
					// This needs to happen after the animation has completed and
					// the final scroll event fired
					setTimeout(function () {
						that.s.ani = false;
					}, 250);
				}
			);
		}
		else {
			$(this.dom.scroller).scrollTop(px);
		}
	},

	/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
	 * Constructor
	 */

	/**
	 * Initialisation for Scroller
	 *  @returns {void}
	 *  @private
	 */
	construct: function () {
		var that = this;
		var dt = this.s.dtApi;

		/* Sanity check */
		if (!this.s.dt.oFeatures.bPaginate) {
			throw new Error(
				'Pagination must be enabled for Scroller to operate'
			);
		}

		/* Insert a div element that we can use to force the DT scrolling container to
		 * the height that would be required if the whole table was being displayed
		 */
		this.dom.force.style.position = 'relative';
		this.dom.force.style.top = '0px';
		this.dom.force.style.left = '0px';
		this.dom.force.style.width = '1px';

		this.dom.scroller = dt.table().node().parentNode;
		this.dom.scroller.appendChild(this.dom.force);
		this.dom.scroller.style.position = 'relative';

		this.dom.table = $('>table', this.dom.scroller)[0];
		this.dom.table.style.position = 'absolute';
		this.dom.table.style.top = '0px';
		this.dom.table.style.left = '0px';

		// Add class to 'announce' that we are a Scroller table
		$(dt.table().container()).addClass('dts DTS');

		this.dom.label.appendTo(this.dom.scroller);

		/* Initial size calculations */
		if (this.s.heights.row && this.s.heights.row != 'auto') {
			this.s.autoHeight = false;
		}

		// Scrolling callback to see if a page change is needed
		this.s.ingnoreScroll = true;
		$(this.dom.scroller).on('scroll.dt-scroller', function (e) {
			that._scroll.call(that);
		});

		// In iOS we catch the touchstart event in case the user tries to scroll
		// while the display is already scrolling
		$(this.dom.scroller).on('touchstart.dt-scroller', function () {
			that._scroll.call(that);
		});

		$(this.dom.scroller)
			.on('mousedown.dt-scroller', function () {
				that.s.mousedown = true;
			})
			.on('mouseup.dt-scroller', function () {
				that.s.labelVisible = false;
				that.s.mousedown = false;
				that.dom.label.css('display', 'none');
			});

		// On resize, update the information element, since the number of rows shown might change
		$(window).on('resize.dt-scroller', function () {
			that.measure(false);
			that._info();
		});

		// Add a state saving parameter to the DT state saving so we can restore the exact
		// position of the scrolling.
		var initialStateSave = true;
		var loadedState = dt.state.loaded();

		dt.on('stateSaveParams.scroller', function (e, settings, data) {
			if (initialStateSave && loadedState) {
				data.scroller = loadedState.scroller;
				initialStateSave = false;

				if (data.scroller) {
					that.s.lastScrollTop = data.scroller.scrollTop;
				}
			}
			else {
				// Need to used the saved position on init
				data.scroller = {
					topRow: that.s.topRowFloat,
					baseScrollTop: that.s.baseScrollTop,
					baseRowTop: that.s.baseRowTop,
					scrollTop: that.s.lastScrollTop
				};
			}
		});

		dt.on('stateLoadParams.scroller', function (e, settings, data) {
			if (data.scroller !== undefined) {
				that.scrollToRow(data.scroller.topRow);
			}
		});

		if (loadedState && loadedState.scroller) {
			this.s.topRowFloat = loadedState.scroller.topRow;
			this.s.baseScrollTop = loadedState.scroller.baseScrollTop;
			this.s.baseRowTop = loadedState.scroller.baseRowTop;
		}

		this.measure(false);

		that.s.stateSaveThrottle = DataTable.util.throttle(function () {
			that.s.dtApi.state.save();
		}, 500);

		dt.on('init.scroller', function () {
			that.measure(false);

			// Setting to `jump` will instruct _draw to calculate the scroll top
			// position
			that.s.scrollType = 'jump';
			that._draw();

			// Update the scroller when the DataTable is redrawn
			dt.on('draw.scroller', function () {
				that._draw();
			});
		});

		// Set height before the draw happens, allowing everything else to update
		// on draw complete without worry for roder.
		dt.on('preDraw.dt.scroller', function () {
			that._scrollForce();
		});

		// Destructor
		dt.on('destroy.scroller', function () {
			$(window).off('resize.dt-scroller');
			$(that.dom.scroller).off('.dt-scroller');
			$(that.s.dt.nTable).off('.scroller');

			$(that.s.dt.nTableWrapper).removeClass('DTS');
			$('div.DTS_Loading', that.dom.scroller.parentNode).remove();

			that.dom.table.style.position = '';
			that.dom.table.style.top = '';
			that.dom.table.style.left = '';
		});
	},

	/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
	 * Private methods
	 */

	/**
	 * Automatic calculation of table row height. This is just a little tricky here as using
	 * initialisation DataTables has tale the table out of the document, so we need to create
	 * a new table and insert it into the document, calculate the row height and then whip the
	 * table out.
	 *  @returns {void}
	 *  @private
	 */
	_calcRowHeight: function () {
		var dt = this.s.dt;
		var origTable = dt.nTable;
		var nTable = origTable.cloneNode(false);
		var tbody = $('<tbody/>').appendTo(nTable);
		var dtClasses = dt.oClasses;

		// Different locations for classes in DT2
		var classes = DataTable.versionCheck('2')
			? {
					container: dtClasses.container,
					scroller: dtClasses.scrolling.container,
					body: dtClasses.scrolling.body
			}
			: {
					container: dtClasses.sWrapper,
					scroller: dtClasses.sScrollWrapper,
					body: dtClasses.sScrollBody
			};

		var container = $(
			'<div class="' +
				classes.container +
				' DTS"><div class="' +
				classes.scroller +
				'"><div class="' +
				classes.body +
				'"></div></div></div>'
		);

		// Want 3 rows in the sizing table so :first-child and :last-child
		// CSS styles don't come into play - take the size of the middle row
		$('tbody tr:lt(4)', origTable).clone().appendTo(tbody);
		var rowsCount = $('tr', tbody).length;

		if (rowsCount === 1) {
			tbody.prepend('<tr><td>&#160;</td></tr>');
			tbody.append('<tr><td>&#160;</td></tr>');
		}
		else {
			for (; rowsCount < 3; rowsCount++) {
				tbody.append('<tr><td>&#160;</td></tr>');
			}
		}

		$('div.' + classes.body, container).append(nTable);

		// If initialised using `dom`, use the holding element as the insert point
		var insertEl = this.s.dt.nHolding || origTable.parentNode;

		if (!$(insertEl).is(':visible')) {
			insertEl = 'body';
		}

		// Remove form element links as they might select over others (particularly radio and checkboxes)
		container.find('input').removeAttr('name');

		container.appendTo(insertEl);
		this.s.heights.row = $('tr', tbody).eq(1).outerHeight();

		container.remove();
	},

	/**
	 * Draw callback function which is fired when the DataTable is redrawn. The main function of
	 * this method is to position the drawn table correctly the scrolling container for the rows
	 * that is displays as a result of the scrolling position.
	 *  @returns {void}
	 *  @private
	 */
	_draw: function () {
		var that = this,
			heights = this.s.heights,
			iScrollTop = this.dom.scroller.scrollTop,
			iTableHeight = $(this.s.dt.nTable).height(),
			displayStart = this.s.dt._iDisplayStart,
			displayLen = this.s.dt._iDisplayLength,
			displayEnd = this.s.dt.fnRecordsDisplay(),
			viewportEndY = iScrollTop + heights.viewport;

		// Disable the scroll event listener while we are updating the DOM
		this.s.skip = true;

		// If paging is reset
		if (
			(this.s.dt.bSorted || this.s.dt.bFiltered) &&
			displayStart === 0 &&
			!this.s.dt._drawHold
		) {
			this.s.topRowFloat = 0;
		}

		iScrollTop =
			this.s.scrollType === 'jump'
				? this._domain(
						'virtualToPhysical',
						this.s.topRowFloat * heights.row
				)
				: iScrollTop;

		// Store positional information so positional calculations can be based
		// upon the current table draw position
		this.s.baseScrollTop = iScrollTop;
		this.s.baseRowTop = this.s.topRowFloat;

		// Position the table in the virtual scroller
		var tableTop =
			iScrollTop - (this.s.topRowFloat - displayStart) * heights.row;
		if (displayStart === 0) {
			tableTop = 0;
		}
		else if (displayStart + displayLen >= displayEnd) {
			tableTop = heights.scroll - iTableHeight;
		}
		else {
			var iTableBottomY = tableTop + iTableHeight;
			if (iTableBottomY < viewportEndY) {
				// The last row of the data is above the end of the viewport.
				// This means the background is visible, which is not what the user expects.
				var newTableTop = viewportEndY - iTableHeight;
				var diffPx = newTableTop - tableTop;
				this.s.baseScrollTop += diffPx + 1; // Update start row number in footer.
				tableTop = newTableTop; // Move table so last line of data is at the bottom of the viewport.
			}
		}

		this.dom.table.style.top = tableTop + 'px';

		/* Cache some information for the scroller */
		this.s.tableTop = tableTop;
		this.s.tableBottom = iTableHeight + this.s.tableTop;

		// Calculate the boundaries for where a redraw will be triggered by the
		// scroll event listener
		var boundaryPx = (iScrollTop - this.s.tableTop) * this.s.boundaryScale;
		this.s.redrawTop = iScrollTop - boundaryPx;
		this.s.redrawBottom =
			iScrollTop + boundaryPx >
			heights.scroll - heights.viewport - heights.row
				? heights.scroll - heights.viewport - heights.row
				: iScrollTop + boundaryPx;

		this.s.skip = false;

		if (that.s.ingnoreScroll) {
			// Restore the scrolling position that was saved by DataTable's state
			// saving Note that this is done on the second draw when data is Ajax
			// sourced, and the first draw when DOM soured
			if (
				this.s.dt.oFeatures.bStateSave &&
				this.s.dt.oLoadedState !== null &&
				typeof this.s.dt.oLoadedState.scroller != 'undefined'
			) {
				// A quirk of DataTables is that the draw callback will occur on an
				// empty set if Ajax sourced, but not if server-side processing.
				var ajaxSourced =
					(this.s.dt.sAjaxSource || that.s.dt.ajax) &&
					!this.s.dt.oFeatures.bServerSide
						? true
						: false;

				if (
					(ajaxSourced && this.s.dt.iDraw >= 2) ||
					(!ajaxSourced && this.s.dt.iDraw >= 1)
				) {
					setTimeout(function () {
						$(that.dom.scroller).scrollTop(
							that.s.dt.oLoadedState.scroller.scrollTop
						);

						// In order to prevent layout thrashing we need another
						// small delay
						setTimeout(function () {
							that.s.ingnoreScroll = false;
						}, 0);
					}, 0);
				}
			}
			else {
				that.s.ingnoreScroll = false;
			}
		}

		// Because of the order of the DT callbacks, the info update will
		// take precedence over the one we want here. So a 'thread' break is
		// needed.  Only add the thread break if bInfo is set
		if (this.s.dt.oFeatures.bInfo) {
			setTimeout(function () {
				that._info.call(that);
			}, 0);
		}

		$(this.s.dt.nTable).triggerHandler('position.dts.dt', tableTop);
	},

	/**
	 * Convert from one domain to another. The physical domain is the actual
	 * pixel count on the screen, while the virtual is if we had browsers which
	 * had scrolling containers of infinite height (i.e. the absolute value)
	 *
	 *  @param {string} dir Domain transform direction, `virtualToPhysical` or
	 *    `physicalToVirtual`
	 *  @returns {number} Calculated transform
	 *  @private
	 */
	_domain: function (dir, val) {
		var heights = this.s.heights;
		var diff;
		var magic = 10000; // the point at which the non-linear calculations start to happen

		// If the virtual and physical height match, then we use a linear
		// transform between the two, allowing the scrollbar to be linear
		if (heights.virtual === heights.scroll) {
			return val;
		}

		// In the first 10k pixels and the last 10k pixels, we want the scrolling
		// to be linear. After that it can be non-linear. It would be unusual for
		// anyone to mouse wheel through that much.
		if (val < magic) {
			return val;
		}
		else if (
			dir === 'virtualToPhysical' &&
			val >= heights.virtual - magic
		) {
			diff = heights.virtual - val;
			return heights.scroll - diff;
		}
		else if (dir === 'physicalToVirtual' && val >= heights.scroll - magic) {
			diff = heights.scroll - val;
			return heights.virtual - diff;
		}

		// Otherwise, we want a non-linear scrollbar to take account of the
		// redrawing regions at the start and end of the table, otherwise these
		// can stutter badly - on large tables 30px (for example) scroll might
		// be hundreds of rows, so the table would be redrawing every few px at
		// the start and end. Use a simple linear eq. to stop this, effectively
		// causing a kink in the scrolling ratio. It does mean the scrollbar is
		// non-linear, but with such massive data sets, the scrollbar is going
		// to be a best guess anyway
		var m =
			(heights.virtual - magic - magic) /
			(heights.scroll - magic - magic);
		var c = magic - m * magic;

		return dir === 'virtualToPhysical' ? (val - c) / m : m * val + c;
	},

	/**
	 * Update any information elements that are controlled by the DataTable based on the scrolling
	 * viewport and what rows are visible in it. This function basically acts in the same way as
	 * _fnUpdateInfo in DataTables, and effectively replaces that function.
	 *  @returns {void}
	 *  @private
	 */
	_info: function () {
		if (!this.s.dt.oFeatures.bInfo) {
			return;
		}

		var dt = this.s.dt,
			dtApi = this.s.dtApi,
			language = dt.oLanguage,
			info = dtApi.page.info(),
			total = info.recordsDisplay,
			max = info.recordsTotal,
			start = Math.floor(this.s.topRowFloat) + 1,
			possibleEnd = start + Math.floor(this.s.heights.viewport / this.s.heights.row),
			end = possibleEnd > total ? total : possibleEnd,
			result;

		if (total === 0 && total == max) {
			/* Empty record set */
			result = language.sInfoEmpty + language.sInfoPostFix;
		}
		else if (total === 0) {
			// Empty record set after filtering
			result =
				language.sInfoEmpty +
				' ' +
				language.sInfoFiltered +
				language.sInfoPostFix;
		}
		else if (total == max) {
			// Normal record set
			result = language.sInfo + language.sInfoPostFix;
		}
		else {
			// Record set after filtering
			result = language.sInfo;
			' ' + language.sInfoFiltered + language.sInfoPostFix;
		}

		result = this._macros(result, start, end, max, total);

		var callback = language.fnInfoCallback;
		if (callback) {
			result = callback.call(
				dt.oInstance,
				dt,
				start,
				end,
				max,
				total,
				result
			);
		}

		// DT 1.x features
		var n = dt.aanFeatures.i;
		if (typeof n != 'undefined') {
			for (var i = 0, iLen = n.length; i < iLen; i++) {
				$(n[i]).html(result);
			}

			$(dt.nTable).triggerHandler('info.dt');
		}

		// DT 2.x features
		$('div.dt-info', dtApi.table().container()).each(function () {
			$(this).html(result);
			dtApi.trigger('info', [this, result]);
		});
	},

	/**
	 * String replacement for info display. Basically the same as what DataTables does.
	 *
	 * @param {*} str
	 * @param {*} start
	 * @param {*} end
	 * @param {*} max
	 * @param {*} total
	 * @returns Formatted string
	 */
	_macros: function (str, start, end, max, total) {
		var api = this.s.dtApi;
		var settings = this.s.dt;
		var formatter = settings.fnFormatNumber;

		return str
			.replace(/_START_/g, formatter.call(settings, start))
			.replace(/_END_/g, formatter.call(settings, end))
			.replace(/_MAX_/g, formatter.call(settings, max))
			.replace(/_TOTAL_/g, formatter.call(settings, total))
			.replace(/_ENTRIES_/g, api.i18n('entries', ''))
			.replace(/_ENTRIES-MAX_/g, api.i18n('entries', '', max))
			.replace(/_ENTRIES-TOTAL_/g, api.i18n('entries', '', total));
	},

	/**
	 * Parse CSS height property string as number
	 *
	 * An attempt is made to parse the string as a number. Currently supported units are 'px',
	 * 'vh', and 'rem'. 'em' is partially supported; it works as long as the parent element's
	 * font size matches the body element. Zero is returned for unrecognized strings.
	 *  @param {string} cssHeight CSS height property string
	 *  @returns {number} height
	 *  @private
	 */
	_parseHeight: function (cssHeight) {
		var height;
		var matches = /^([+-]?(?:\d+(?:\.\d+)?|\.\d+))(px|em|rem|vh)$/.exec(
			cssHeight
		);

		if (matches === null) {
			return 0;
		}

		var value = parseFloat(matches[1]);
		var unit = matches[2];

		if (unit === 'px') {
			height = value;
		}
		else if (unit === 'vh') {
			height = (value / 100) * $(window).height();
		}
		else if (unit === 'rem') {
			height = value * parseFloat($(':root').css('font-size'));
		}
		else if (unit === 'em') {
			height = value * parseFloat($('body').css('font-size'));
		}

		return height ? height : 0;
	},

	/**
	 * Scrolling function - fired whenever the scrolling position is changed.
	 * This method needs to use the stored values to see if the table should be
	 * redrawn as we are moving towards the end of the information that is
	 * currently drawn or not. If needed, then it will redraw the table based on
	 * the new position.
	 *  @returns {void}
	 *  @private
	 */
	_scroll: function () {
		var that = this,
			heights = this.s.heights,
			iScrollTop = this.dom.scroller.scrollTop,
			iTopRow;

		if (this.s.skip) {
			return;
		}

		if (this.s.ingnoreScroll) {
			return;
		}

		if (iScrollTop === this.s.lastScrollTop) {
			return;
		}

		/* If the table has been sorted or filtered, then we use the redraw that
		 * DataTables as done, rather than performing our own
		 */
		if (this.s.dt.bFiltered || this.s.dt.bSorted) {
			this.s.lastScrollTop = 0;
			return;
		}

		/* We don't want to state save on every scroll event - that's heavy
		 * handed, so use a timeout to update the state saving only when the
		 * scrolling has finished
		 */
		clearTimeout(this.s.stateTO);
		this.s.stateTO = setTimeout(function () {
			that.s.dtApi.state.save();
		}, 250);

		this.s.scrollType =
			Math.abs(iScrollTop - this.s.lastScrollTop) > heights.viewport
				? 'jump'
				: 'cont';

		this.s.topRowFloat =
			this.s.scrollType === 'cont'
				? this.pixelsToRow(iScrollTop, false, false)
				: this._domain('physicalToVirtual', iScrollTop) / heights.row;

		if (this.s.topRowFloat < 0) {
			this.s.topRowFloat = 0;
		}

		/* Check if the scroll point is outside the trigger boundary which would required
		 * a DataTables redraw
		 */
		if (
			this.s.forceReposition ||
			iScrollTop < this.s.redrawTop ||
			iScrollTop > this.s.redrawBottom
		) {
			var preRows = Math.ceil(
				((this.s.displayBuffer - 1) / 2) * this.s.viewportRows
			);

			iTopRow = parseInt(this.s.topRowFloat, 10) - preRows;
			this.s.forceReposition = false;

			if (iTopRow <= 0) {
				/* At the start of the table */
				iTopRow = 0;
			}
			else if (
				iTopRow + this.s.dt._iDisplayLength >
				this.s.dt.fnRecordsDisplay()
			) {
				/* At the end of the table */
				iTopRow =
					this.s.dt.fnRecordsDisplay() - this.s.dt._iDisplayLength;
				if (iTopRow < 0) {
					iTopRow = 0;
				}
			}
			else if (iTopRow % 2 !== 0) {
				// For the row-striping classes (odd/even) we want only to start
				// on evens otherwise the stripes will change between draws and
				// look rubbish
				iTopRow++;
			}

			// Store calcuated value, in case the following condition is not met, but so
			// that the draw function will still use it.
			this.s.targetTop = iTopRow;

			if (iTopRow != this.s.dt._iDisplayStart) {
				/* Cache the new table position for quick lookups */
				this.s.tableTop = $(this.s.dt.nTable).offset().top;
				this.s.tableBottom =
					$(this.s.dt.nTable).height() + this.s.tableTop;

				var draw = function () {
					that.s.dt._iDisplayStart = that.s.targetTop;
					that.s.dtApi.draw('page');
				};

				/* Do the DataTables redraw based on the calculated start point - note that when
				 * using server-side processing we introduce a small delay to not DoS the server...
				 */
				if (this.s.dt.oFeatures.bServerSide) {
					this.s.forceReposition = true;

					// This is used only for KeyTable and is not currently publicly
					// documented. Open question - is it useful for anything else?
					$(this.s.dt.nTable).triggerHandler('scroller-will-draw.dt');

					if (DataTable.versionCheck('2')) {
						that.s.dtApi.processing(true);
					}
					else {
						this.s.dt.oApi._fnProcessingDisplay(this.s.dt, true);
					}

					clearTimeout(this.s.drawTO);
					this.s.drawTO = setTimeout(draw, this.s.serverWait);
				}
				else {
					draw();
				}
			}
		}
		else {
			this.s.topRowFloat = this.pixelsToRow(iScrollTop, false, true);
		}

		/* Update the table's information display for what is now in the viewport */
		this._info();

		this.s.lastScrollTop = iScrollTop;
		this.s.stateSaveThrottle();

		if (this.s.scrollType === 'jump' && this.s.mousedown) {
			this.s.labelVisible = true;
		}
		if (this.s.labelVisible) {
			var labelFactor =
				(heights.viewport - heights.labelHeight - heights.xbar) /
				heights.scroll;

			this.dom.label
				.html(
					this.s.dt.fnFormatNumber(
						parseInt(this.s.topRowFloat, 10) + 1
					)
				)
				.css('top', iScrollTop + iScrollTop * labelFactor)
				.css('display', 'block');
		}
	},

	/**
	 * Force the scrolling container to have height beyond that of just the
	 * table that has been drawn so the user can scroll the whole data set.
	 *
	 * Note that if the calculated required scrolling height exceeds a maximum
	 * value (1 million pixels - hard-coded) the forcing element will be set
	 * only to that maximum value and virtual / physical domain transforms will
	 * be used to allow Scroller to display tables of any number of records.
	 *  @returns {void}
	 *  @private
	 */
	_scrollForce: function () {
		var heights = this.s.heights;
		var max = 1000000;

		heights.virtual = heights.row * this.s.dt.fnRecordsDisplay();
		heights.scroll = heights.virtual;

		if (heights.scroll > max) {
			heights.scroll = max;
		}

		// Minimum height so there is always a row visible (the 'no rows found'
		// if reduced to zero filtering)
		this.dom.force.style.height =
			heights.scroll > this.s.heights.row
				? heights.scroll + 'px'
				: this.s.heights.row + 'px';
	}
});

/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Statics
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

/**
 * Scroller default settings for initialisation
 *  @namespace
 *  @name Scroller.defaults
 *  @static
 */
Scroller.defaults = {
	/**
	 * Scroller uses the boundary scaling factor to decide when to redraw the table - which it
	 * typically does before you reach the end of the currently loaded data set (in order to
	 * allow the data to look continuous to a user scrolling through the data). If given as 0
	 * then the table will be redrawn whenever the viewport is scrolled, while 1 would not
	 * redraw the table until the currently loaded data has all been shown. You will want
	 * something in the middle - the default factor of 0.5 is usually suitable.
	 *  @type     float
	 *  @default  0.5
	 *  @static
	 */
	boundaryScale: 0.5,

	/**
	 * The display buffer is what Scroller uses to calculate how many rows it should pre-fetch
	 * for scrolling. Scroller automatically adjusts DataTables' display length to pre-fetch
	 * rows that will be shown in "near scrolling" (i.e. just beyond the current display area).
	 * The value is based upon the number of rows that can be displayed in the viewport (i.e.
	 * a value of 1), and will apply the display range to records before before and after the
	 * current viewport - i.e. a factor of 3 will allow Scroller to pre-fetch 1 viewport's worth
	 * of rows before the current viewport, the current viewport's rows and 1 viewport's worth
	 * of rows after the current viewport. Adjusting this value can be useful for ensuring
	 * smooth scrolling based on your data set.
	 *  @type     int
	 *  @default  7
	 *  @static
	 */
	displayBuffer: 9,

	/**
	 * Scroller will attempt to automatically calculate the height of rows for it's internal
	 * calculations. However the height that is used can be overridden using this parameter.
	 *  @type     int|string
	 *  @default  auto
	 *  @static
	 */
	rowHeight: 'auto',

	/**
	 * When using server-side processing, Scroller will wait a small amount of time to allow
	 * the scrolling to finish before requesting more data from the server. This prevents
	 * you from DoSing your own server! The wait time can be configured by this parameter.
	 *  @type     int
	 *  @default  200
	 *  @static
	 */
	serverWait: 200
};

Scroller.oDefaults = Scroller.defaults;

/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Constants
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

/**
 * Scroller version
 *  @type      String
 *  @default   See code
 *  @name      Scroller.version
 *  @static
 */
Scroller.version = '2.3.0';

/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Initialisation
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

// Attach a listener to the document which listens for DataTables initialisation
// events so we can automatically initialise
$(document).on('preInit.dt.dtscroller', function (e, settings) {
	if (e.namespace !== 'dt') {
		return;
	}

	var init = settings.oInit.scroller;
	var defaults = DataTable.defaults.scroller;

	if (init || defaults) {
		var opts = $.extend({}, init, defaults);

		if (init !== false) {
			new Scroller(settings, opts);
		}
	}
});

// Attach Scroller to DataTables so it can be accessed as an 'extra'
$.fn.dataTable.Scroller = Scroller;
$.fn.DataTable.Scroller = Scroller;

// DataTables 1.10 API method aliases
var Api = $.fn.dataTable.Api;

Api.register('scroller()', function () {
	return this;
});

// Undocumented and deprecated - is it actually useful at all?
Api.register('scroller().rowToPixels()', function (rowIdx, intParse, virtual) {
	var ctx = this.context;

	if (ctx.length && ctx[0].oScroller) {
		return ctx[0].oScroller.rowToPixels(rowIdx, intParse, virtual);
	}
	// undefined
});

// Undocumented and deprecated - is it actually useful at all?
Api.register('scroller().pixelsToRow()', function (pixels, intParse, virtual) {
	var ctx = this.context;

	if (ctx.length && ctx[0].oScroller) {
		return ctx[0].oScroller.pixelsToRow(pixels, intParse, virtual);
	}
	// undefined
});

// `scroller().scrollToRow()` is undocumented and deprecated. Use `scroller.toPosition()
Api.register(
	['scroller().scrollToRow()', 'scroller.toPosition()'],
	function (idx, ani) {
		this.iterator('table', function (ctx) {
			if (ctx.oScroller) {
				ctx.oScroller.scrollToRow(idx, ani);
			}
		});

		return this;
	}
);

Api.register('row().scrollTo()', function (ani) {
	var that = this;

	this.iterator('row', function (ctx, rowIdx) {
		if (ctx.oScroller) {
			var displayIdx = that
				.rows({ order: 'applied', search: 'applied' })
				.indexes()
				.indexOf(rowIdx);

			ctx.oScroller.scrollToRow(displayIdx, ani);
		}
	});

	return this;
});

Api.register('scroller.measure()', function (redraw) {
	this.iterator('table', function (ctx) {
		if (ctx.oScroller) {
			ctx.oScroller.measure(redraw);
		}
	});

	return this;
});

Api.register('scroller.page()', function () {
	var ctx = this.context;

	if (ctx.length && ctx[0].oScroller) {
		return ctx[0].oScroller.pageInfo();
	}
	// undefined
});


return DataTable;
}));


